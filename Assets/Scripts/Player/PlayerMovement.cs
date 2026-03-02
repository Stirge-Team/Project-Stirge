using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Strige.Camera;

namespace Strige.Player {
public class PlayerMovement : MonoBehaviour
{
  [System.Serializable]
  private struct stateVariables
  {
    [Tooltip("The rate at which the player moves around. This value is applied when an input is given by the player. DO NOT set this value far greater then the maximum speed, this will cause the player to jump around.")]
    public float _horizontalAcceleration;
    [Tooltip("The maximum velocity the player can reach. Once reaching this speed, more force will not be applied in the player's current direction of travel.")]
    public float _maximumHorizontalSpeed;
    [Tooltip("The speed at which the player turns around to face the given input direction")]
    public float _rotationSpeed;
    [Tooltip("A constant force applied to the player. It works to reduce the player's speed down to zero.")]
    public float _friction;
    [Tooltip("Scales the user input.")]
    public AnimationCurve _inputStrength;
  }
  //The input from the player
	private Vector2 m_inputDirection = new Vector2();
  //I bet you can't guess what this one is for
	private Rigidbody m_playerBody;
	[Header("Horizontal Movement Settings")]
  [SerializeField, Tooltip("These are the values that will be used while the player is grounded")]
  private stateVariables m_groundSettings;
  [SerializeField, Tooltip("These are the values that will be used while the player is in the air.")]
  private stateVariables m_aerialSettings;
  //Selector for the settings
  private stateVariables m_currentStateSettings => IsGrounded ? m_groundSettings : m_aerialSettings;

	[Header("Jump Settings")]
	[SerializeField, Tooltip("The desired height you'd like the player to reach.")]
	private float m_jumpHeight = 5f;
  //Grounded bool
	public bool IsGrounded {get; private set;}
  [SerializeField, Tooltip("The distance from the center of the player that considers them grounded. This uses a sphere with a radius of 0.5f.")]
  private float m_groundCheckDistance;
  //The layers that the player considers "ground"
  private LayerMask m_groundCheckLayers;
	[SerializeField, Tooltip("The window after falling off an object that the player can still jump.")]
	private float m_coyoteTime = 0.2f;
  //The remaining time for coyote time
	private float m_coyoteCountdown;
  [Header("Fall Speed")]
  [SerializeField, Tooltip("The maximum speed the player can fall (0 will skip this check"), Min(0)]
  private float m_fallSpeedCap = 0;
  [SerializeField, Tooltip("How much the time the player has spent falling should affect their falling speed.")]
  private float m_fallTimeSpeedMultiplier = 0;
  //The time the player has been falling
  private float m_currentFallTime = 0;
  //The maximum height the player reached - used for the fall speed mult check
  private float m_lastCheckedHeight = 0;

	private Transform m_cameraTransform;
	

	void Start()
	{
		//Gets all relivant components
		m_playerBody = GetComponent<Rigidbody>();
		m_cameraTransform = UnityEngine.Camera.main.transform;
		//Sets layers for the ground check - may make this changable in future
		m_groundCheckLayers = LayerMask.GetMask("Ground");
	}

	// Update is called once per frame
	void FixedUpdate()
	{
    //Calculates the direction to move the player in given the current inputs and camera transform
    Vector3 attemptedMoveDirection = (new Vector3(m_cameraTransform.forward.x, 0, m_cameraTransform.forward.z) * m_inputDirection.y + new Vector3(m_cameraTransform.right.x, 0, m_cameraTransform.right.z) * m_inputDirection.x).normalized;
    //Only when the player applies any directional inputs...
    if(attemptedMoveDirection.magnitude > 0)
    {
      //Interperlate the rotations between the current player rotation and the given input direction
      transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attemptedMoveDirection), m_currentStateSettings._rotationSpeed);
    }

    //Stop the player from looking at the ground
    transform.Rotate(-transform.rotation.x, 0, 0);

    //Get the player's horizontal velocity
    Vector3 playerBodyHorizontalVelocity = new Vector3 (m_playerBody.linearVelocity.x, 0, m_playerBody.linearVelocity.z);

    Debug.DrawRay(transform.position, playerBodyHorizontalVelocity, Color.blue);
    Debug.DrawRay(transform.position, attemptedMoveDirection, Color.red);

    //If the player's current horizontal velocity is less then the speed limit, then the player can be moved
    if(playerBodyHorizontalVelocity.magnitude < m_currentStateSettings._maximumHorizontalSpeed || Vector3.Angle(playerBodyHorizontalVelocity.normalized, attemptedMoveDirection) > 90.0f)
    {
      //Apply the force to the player
      m_playerBody.AddForce(m_currentStateSettings._inputStrength.Evaluate(m_inputDirection.magnitude) * m_inputDirection.magnitude * transform.forward * m_currentStateSettings._horizontalAcceleration * Time.deltaTime);
    }

    //do some decceleration - the clamped value helps when getting the movement down to zero
    m_playerBody.AddForce(playerBodyHorizontalVelocity.normalized * -m_currentStateSettings._friction * Mathf.Clamp(playerBodyHorizontalVelocity.magnitude, 0, 1) * Time.deltaTime);
    
    //Clamping the players fall speed
    if(m_fallSpeedCap > 0 && -m_fallSpeedCap > m_playerBody.linearVelocity.y)
    {
      m_playerBody.linearVelocity -= new Vector3(0, m_playerBody.linearVelocity.y + m_fallSpeedCap, 0); 
    }
		
    //Casts a sphere down from the player's center, and outs true if a ground layer object is hit
		if(Physics.CheckSphere(transform.position + Vector3.down * m_groundCheckDistance, 0.5f, m_groundCheckLayers))
		{
			//Check if the player is not considered grounded
			if(!IsGrounded)
			{
				//Make them grounded
				IsGrounded = true;
				//Reset the coyote time
				m_coyoteCountdown = m_coyoteTime;
        //Reset the fall time
        m_currentFallTime = 0;
			}
		}
		else
		{
			//Check if there is remaining coyote time
			if(m_coyoteCountdown > 0 && IsGrounded)
			{
				//Continue the countdown
				m_coyoteCountdown -= Time.deltaTime;
			}
			//When there is no more coyote time
			if(m_coyoteCountdown <= 0 && IsGrounded)
			{
				//The player is no longer grounded
				IsGrounded = false;
        //Reset the last checked height
        m_lastCheckedHeight = transform.position.y;
			}

      if(!IsGrounded)
      {
        //First determine if the player is falling
        if(m_lastCheckedHeight > transform.position.y)
        {
          //Start timing the player's fall
          m_currentFallTime += Time.deltaTime;
        
          //Add a little more force to the player when they have been falling for a while.
          m_playerBody.AddForce(-transform.up * m_currentFallTime * m_fallTimeSpeedMultiplier);
        }
        //Update the last height if the player is height than before
        else if(m_lastCheckedHeight < transform.position.y)
        {
          m_lastCheckedHeight = transform.position.y;
        }
      }
		}
	}

	//Called from the player input component - updates the input direction value
	public void OnMove(InputValue value)
	{
		m_inputDirection = value.Get<Vector2>();
	}

	public void OnJump(InputValue value)
	{
		//If the player is considered grounded
		if (IsGrounded)
		{
      m_playerBody.linearVelocity = new Vector3(m_playerBody.linearVelocity.x ,0, m_playerBody.linearVelocity.z);
			//Apply a force up on the player
			m_playerBody.AddForce(transform.up * Mathf.Sqrt(2 * m_jumpHeight * -Physics.gravity.y), ForceMode.Impulse);
			//Remove all coyote time 
			m_coyoteCountdown = 0;
			//Grounded is not set to off here as the first check in fixed update will reset the player to being grounded in this frame
		}
	}

  public void OnDrawGizmos()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, m_currentStateSettings._maximumHorizontalSpeed);

    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, (m_currentStateSettings._horizontalAcceleration - m_currentStateSettings._friction) * 0.0166f);

    Gizmos.color = Color.purple;
    Gizmos.DrawSphere(transform.position + -transform.up * m_groundCheckDistance, 0.5f);
    switch(IsGrounded)
    {
      case true:
        Gizmos.DrawSphere(transform.position + transform.up * m_jumpHeight, 0.5f);
        break;
      case false:
        Gizmos.DrawSphere(transform.position + transform.up * (m_jumpHeight - m_lastCheckedHeight), 0.5f);
        break;
    }

    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(transform.position - transform.up * m_fallSpeedCap, Vector3.one);
    if(m_playerBody)
    Gizmos.DrawRay(transform.position, transform.up * Mathf.Clamp(m_playerBody.linearVelocity.y, -Mathf.Infinity, 0));

    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position + transform.up * 2 , m_coyoteTime);
    Gizmos.color = Color.orange;
    Gizmos.DrawWireSphere(transform.position + transform.up * 2, m_coyoteCountdown);
  public void OnLook(InputValue value)
  {
    Object.FindObjectOfType<TrackingCamera>().OnLook(value.Get<Vector2>());
  }
}}
