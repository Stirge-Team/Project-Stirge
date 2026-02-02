using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 m_inputDirection = new Vector2();
    private Rigidbody m_playerBody;
    [Header("Horizontal Movement")]
    [SerializeField] private float m_movementSpeed;

    [Header("Vertical Movement")]
    [SerializeField, Tooltip("The amount of force used to push the player object up.")] private float m_jumpForce;
    public bool IsGrounded {get; private set;}
    [SerializeField, Tooltip("The window after falling that the player can still jump.")] private float m_coyoteTime;
    private float m_coyoteCountdown;
    [SerializeField, Tooltip("The distance from the center of the player that considers them grounded.")] private float m_groundCheckDistance;
    private LayerMask m_groundCheckLayers;

    private Transform m_cameraTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      m_playerBody = GetComponent<Rigidbody>();
      m_cameraTransform = Camera.main.transform;

      m_groundCheckLayers = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      Vector3 attemptedMoveDirection = (new Vector3(m_cameraTransform.forward.x, 0, m_cameraTransform.forward.z) * m_inputDirection.y + new Vector3(m_cameraTransform.right.x, 0, m_cameraTransform.right.z) * m_inputDirection.x) * m_movementSpeed * Time.deltaTime;
      m_playerBody.Move(transform.position + attemptedMoveDirection, Quaternion.identity);
    
      RaycastHit hit;
      if(Physics.Raycast(transform.position, Vector3.down, out hit, m_groundCheckDistance, m_groundCheckLayers))
      {
        Debug.DrawRay(transform.position, Vector3.down * m_groundCheckDistance, Color.green);
        if(!IsGrounded)
        {
          IsGrounded = true;
          m_coyoteCountdown = m_coyoteTime;
        }
      }
      else
      {
        Debug.DrawRay(transform.position, Vector3.down * m_groundCheckDistance, Color.red);
        if(m_coyoteCountdown > 0 && IsGrounded)
        {
          m_coyoteCountdown -= Time.deltaTime;
        }
        if(m_coyoteCountdown <= 0 && IsGrounded)
        {
          IsGrounded = false;
        }
      }
    }

    public void OnMove(InputValue value)
    {
      m_inputDirection = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
      if (IsGrounded)
      {
        m_playerBody.AddForce(transform.up * m_jumpForce);
        m_coyoteCountdown = 0;
      }
    }
}
