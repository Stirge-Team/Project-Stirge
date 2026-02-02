using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 m_inputDirection = new Vector2();
    private Rigidbody m_playerBody;
    [SerializeField] private float m_movementSpeed;
    [SerializeField] private float m_jumpForce;
    
    public bool IsGrounded {get; private set;}
    [SerializeField] private float m_coyoteTime;
    private float m_coyoteCountdown;
    [SerializeField] private float m_groundCheckDistance;
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
    void Update()
    {
      m_playerBody.AddForce((new Vector3(m_cameraTransform.forward.x, 0, m_cameraTransform.forward.z) * m_inputDirection.y + new Vector3(m_cameraTransform.right.x, 0, m_cameraTransform.right.z) * m_inputDirection.x) * m_movementSpeed);
    
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
