using UnityEngine;
using UnityEngine.InputSystem;

namespace Strige.Player {
public class PlayerMovement : MonoBehaviour
{
	private Vector2 m_inputDirection = new Vector2();
	private Rigidbody m_playerBody;
	[Header("Horizontal Movement")]
  [SerializeField, Tooltip("The rate at which the player's horizontal velocity increases on the ground.")]
	private float m_horizontalGroundedAccel;
  [SerializeField, Tooltip("The rate at which the player's horizontal velocity increases in the air.")]
  private float m_horizontalAerialAccel;
  [SerializeField, Tooltip("The maximum units the player can move.")]
  private float m_maxHorizontalVelocity;
  [SerializeField, Tooltip("The degrees (in euler) that the player object rotates")]
  private float m_rotationSpeed;


	[Header("Vertical Movement")]
	[SerializeField, Tooltip("The amount of force used to push the player object up.")]
	private float m_jumpForce;
	public bool IsGrounded {get; private set;}
	[SerializeField, Tooltip("The window after falling that the player can still jump.")]
	private float m_coyoteTime;
	private float m_coyoteCountdown;
	[SerializeField, Tooltip("The distance from the center of the player that considers them grounded.")]
	private float m_groundCheckDistance;
	private LayerMask m_groundCheckLayers;


	private Transform m_cameraTransform;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
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
    //Rotates the player object to face the given direction
    //Only when the player applies any directional inputs...
    if(attemptedMoveDirection.magnitude > 0)
    {
      //Interperlate the rotations between the current player rotation and the given input direction
      transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attemptedMoveDirection), m_rotationSpeed);
    }
    //Quaternion rotationDirDelta = transform.rotation * lookDir;
    //transform.rotation = Quaternion.Lerp(transform.rotation, lookDir, rotationDirDelta.magnitude);
    //transform.LookAt(transform.position + attemptedMoveDirection); //change to a lerp
    //Resets the rotation so the player object isn't facing down
    transform.Rotate(-transform.rotation.x, 0, 0);
    //Setup the force that will be applied
    Vector3 moveForce = m_inputDirection.magnitude * transform.forward * ((IsGrounded) ? m_horizontalGroundedAccel : m_horizontalAerialAccel);

    //Finds the player's current horizontal velocity
    Vector3 playerBodyHorizontalVelocity = new Vector3 (m_playerBody.linearVelocity.x, 0, m_playerBody.linearVelocity.z);
    //Estimate what the resulting velocity will be - IDK how acurate this is but it works so don't touch it
    Vector3 estimVelocityDelta = (playerBodyHorizontalVelocity + moveForce * Time.deltaTime);
    if(estimVelocityDelta.magnitude < m_maxHorizontalVelocity)
    {
      //Applies the calculated movement to the players current position.
      m_playerBody.AddForce(moveForce);
    }
		
		//Ground check
		RaycastHit hit;
		//Casts a ray down from the player's center, and outs true if a ground layer object is hit
		if(Physics.Raycast(transform.position, Vector3.down, out hit, m_groundCheckDistance, m_groundCheckLayers))
		{
			Debug.DrawRay(transform.position, Vector3.down * m_groundCheckDistance, Color.green);
			//Check if the player is not considered grounded
			if(!IsGrounded)
			{
				//Make them grounded
				IsGrounded = true;
				//Reset the coyote time
				m_coyoteCountdown = m_coyoteTime;
			}
		}
		else
		{
			Debug.DrawRay(transform.position, Vector3.down * m_groundCheckDistance, Color.red);
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
			//Apply a force up on the player
			m_playerBody.AddForce(transform.up * m_jumpForce);
			//Remove all coyote time 
			m_coyoteCountdown = 0;
			//Grounded is not set to off here as the first check in fixed update will reset the player to being grounded in this frame
		}
	}
}}
