using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Stirge.Combat
{
    public enum MotorMovementState
    {
        Force,
        Velocity,
        Kinematic,
        Navigation,
        Action
    }

    public abstract class CombatEntityMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_transform;

        [Header("Components")]
        [SerializeField] private Rigidbody m_rb;
        [SerializeField] private Collider m_col;

        [Header("Movement Fields")]
        [SerializeField] private MotorMovementState m_defaultMovementState = MotorMovementState.Velocity;
        [SerializeField] private LayerMask m_walkableLayers;
        [SerializeField] private MovementProperties m_groundedMovementProperties;
        [SerializeField] private MovementProperties m_aerialMovementProperties;
        [SerializeField, Min(0f)] private float m_verticalTopSpeed;
        [SerializeField, Min(0f)] private float m_maxFallingSpeed;
        [SerializeField, Min(0f)] private float m_coyoteTime;
        [SerializeField, Min(0f)] private float m_fallSpeedMultiplier;

        [Header("Grounded Check")]
        [SerializeField, Min(0)] private float m_groundCheckDistance;
        [SerializeField, Min(0)] private float m_groundCheckRadius;
        [SerializeField, Range(0f, 90f)] private float m_slopeLimit;

        // velocities
        protected Vector3 m_currentVelocity;
        private Vector3 m_feetOffset;

        // heading
        protected Vector3 m_targetHeading;
        private Transform m_lookTarget;

        // movement state
        private MotorMovementState m_movementState = MotorMovementState.Kinematic;
        private float m_coyoteCountdown;
        private float m_airTime;
        private bool m_isGrounded;
        private readonly RaycastHit[] m_groundedCheckHits = new RaycastHit[10];

        // reference properties
        public new Transform transform => m_transform;
        public Rigidbody Rigidbody => m_rb;
        public Collider Collider => m_col;

        // protected properties
        protected MovementProperties groundedMovementProperties => m_groundedMovementProperties;
        protected MovementProperties aerialMovementProperties => m_aerialMovementProperties;
        protected LayerMask walkableLayers => m_walkableLayers;
        protected MotorMovementState defaultMovementState => m_defaultMovementState;
        protected Transform lookTarget => m_lookTarget;
        protected float horizontalTopSpeed => CurrentMovementProperties.HorizontalTopSpeed;
        protected float acceleration => CurrentMovementProperties.Acceleration;
        protected float angularSpeed => CurrentMovementProperties.AngularSpeed;
        protected float friction => CurrentMovementProperties.Friction;
        protected float verticalTopSpeed => m_verticalTopSpeed;
        protected float maxFallingSpeed => m_maxFallingSpeed;
        protected float coyoteCountdown => m_coyoteCountdown;

        // public properties
        public MovementProperties CurrentMovementProperties => m_isGrounded ? m_groundedMovementProperties : m_aerialMovementProperties;
        public MotorMovementState MovementState => m_movementState;
        public bool IsGrounded => m_isGrounded;
        public float AirTime => m_airTime;
        public Vector3 FeetPosition => m_rb.position + m_feetOffset;

        #region Unity Events
        private void Awake()
        {
            m_feetOffset = new(0f, -m_col.bounds.extents.y, 0f);

            OnAwake();
        }
        protected virtual void OnAwake() { }

        private void Start()
        {
            m_rb.isKinematic = false;
            m_rb.useGravity = false;           

            SetMovementState(MotorMovementState.Velocity);

            OnStart();
        }
        protected virtual void OnStart() { }

        private void Update()
        {
            if (!m_isGrounded)
            {
                m_airTime += Time.deltaTime;
            }

            if (m_coyoteCountdown > 0)
            {
                m_coyoteCountdown -= Time.deltaTime;
            }

            OnUpdate();
        }
        protected virtual void OnUpdate() { }

        private void FixedUpdate()
        {
            // only do cast once per physics frame
            m_isGrounded = PerformIsGroundedCheck();

            switch (m_movementState)
            {
                case MotorMovementState.Force:
                    UpdateForce();
                    UpdateHeading();
                    break;
                case MotorMovementState.Velocity:
                    UpdateVelocity();
                    UpdateHeading();
                    break;
                case MotorMovementState.Navigation:
                    UpdateNavigation();
                    break;
                default:
                    break;
            }

            OnFixedUpdate();
        }
        protected virtual void OnFixedUpdate() { }
        #endregion

        #region Transformation
        public void SetPosition(Vector3 newPosition)
        {
            m_rb.MovePosition(newPosition);
            OnSetPosition(newPosition);
        }
        protected virtual void OnSetPosition(Vector3 newPosition) { }
        public void SetRotation(Quaternion newRotation)
        {
            m_rb.MoveRotation(newRotation);
            OnSetRotation();
        }
        protected virtual void OnSetRotation() { }
        public void SetPositionAndRotation(Vector3 newPosition, Quaternion newRotation)
        {
            m_rb.Move(newPosition, newRotation);
            OnSetPositionAndRotation(newPosition, newRotation);
        }
        protected virtual void OnSetPositionAndRotation(Vector3 newPosition, Quaternion newRotation) { }
        #endregion

        #region Physics
        private bool PerformIsGroundedCheck()
        {
            if (!m_col)
                return false;

            // check the area under the Enemy to check for objects on Layers marked in m_walkableLayers
            int hitCount = Physics.BoxCastNonAlloc(
                m_rb.position,
                new Vector3(m_groundCheckRadius, 0.1f, m_groundCheckRadius),
                Vector3.down,
                m_groundedCheckHits,
                m_rb.rotation,
                m_groundCheckDistance,
                m_walkableLayers,
                QueryTriggerInteraction.Ignore);

            // if no hits, not grounded
            if (hitCount == 0)
            {
                // if leaving the ground, start coyote countdown
                if (m_isGrounded)
                {
                    m_coyoteCountdown = m_coyoteTime;
                }
                return false;
            }
            else
            {
                // if changing from not grounded to grounded aka landing
                if (!m_isGrounded)
                {                    
                    m_airTime = 0f;
                    ResetVerticalVelocity();
                }

                // get closest hit walkable object
                RaycastHit closestHit = m_groundedCheckHits[0];
                if (hitCount > 1)
                {
                    float closestDistance = Vector3.Distance(closestHit.point, m_rb.position);
                    for (int i = 1; i < hitCount; i++)
                    {
                        float distance = Vector3.Distance(m_groundedCheckHits[i].point, FeetPosition);
                        if (distance < closestDistance)
                        {
                            closestHit = m_groundedCheckHits[i];
                            closestDistance = distance;
                        }
                    }
                }

                // if the closest object does not meet the slope limit requirements, then the
                // Enemy is NOT standing
                if (Vector3.Angle(closestHit.normal, Vector3.up) > m_slopeLimit)
                    return false;

                return true;
            }
        }

        private void UpdateForce()
        {
            OnPreUpdateForce();

            if (!m_isGrounded)
            {
                m_rb.AddForce(Physics.gravity * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }

            m_rb.linearVelocity = Vector3.ClampMagnitude(m_rb.linearVelocity, horizontalTopSpeed);

            OnPostUpdateForce();
        }
        protected virtual void OnPreUpdateForce() { }
        protected virtual void OnPostUpdateForce() { }

        private void UpdateVelocity()
        {
            OnPreUpdateVelocity();

            if (!m_isGrounded)
            {
                float fallSpeedMultiplier = m_fallSpeedMultiplier * m_airTime;
                if (fallSpeedMultiplier == 0f)
                    fallSpeedMultiplier = 1f;
                m_currentVelocity += fallSpeedMultiplier * Time.fixedDeltaTime * Physics.gravity;
            }

            float verticalvelocity = m_currentVelocity.y;
            verticalvelocity = Mathf.Min(verticalvelocity, verticalvelocity < 0 ? m_maxFallingSpeed : m_verticalTopSpeed);

            Vector2 horizontalVelocity = new(m_currentVelocity.x, m_currentVelocity.z);
            horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity, horizontalTopSpeed);

            m_rb.linearVelocity = new Vector3(horizontalVelocity.x, verticalvelocity, horizontalVelocity.y);

            OnPostUpdateVelocity();
        }
        protected virtual void OnPreUpdateVelocity() { }
        protected virtual void OnPostUpdateVelocity() { }

        private void UpdateHeading()
        {
            OnPreUpdateHeading();

            m_targetHeading.y = 0;

            if (transform.forward != m_targetHeading)
            {
                Vector3 newHeading = Vector3.RotateTowards(transform.forward, m_targetHeading, angularSpeed * Time.fixedDeltaTime, 0f);
                Quaternion newRotation = Quaternion.LookRotation(newHeading);
                m_rb.MoveRotation(newRotation);
            }

            OnPostUpdateHeading();
        }
        protected virtual void OnPreUpdateHeading() { }
        protected virtual void OnPostUpdateHeading() { }

        // Unless we want to add a NavMeshAgent to the player, this only needs to be implemented for the UtilityEnemy.
        // It is defined here so that the FixedUpdate does not need to be overriden to add the Navigation case.
        protected virtual void UpdateNavigation() { }

        public void ResetHorizontalVelocity()
        {
            m_currentVelocity.x = 0;
            m_currentVelocity.z = 0;
        }
        public void ResetVerticalVelocity()
        {
            m_currentVelocity.y = 0;
        }
        public void ResetVelocity()
        {
            m_currentVelocity = Vector3.zero;
        }

        public void SetLookTarget(Transform newTarget)
        {
            m_lookTarget = newTarget;
            OnSetLookTarget(newTarget);
        }
        protected virtual void OnSetLookTarget(Transform newTarget) { }

        public void AddForce(Vector3 force)
        {
            SetMovementState(MotorMovementState.Force);
            m_rb.AddForce(force);
        }
        public void AddVelocity(Vector3 velocity)
        {
            SetMovementState(MotorMovementState.Velocity);
            m_currentVelocity += velocity;
        }

        public void ChangeGroundedMovementProperties(MovementProperties newProperties)
        {
            m_groundedMovementProperties = newProperties;
            OnChangeGroundedMovementProperties(newProperties);
        }
        protected virtual void OnChangeGroundedMovementProperties(MovementProperties newProperties) { }

        public void ChangeAerialMovementProperties(MovementProperties newProperties)
        {
            m_aerialMovementProperties = newProperties;
            OnChangeAerialMovementProperties(newProperties);
        }
        protected virtual void OnChangeAerialMovementProperties(MovementProperties newProperties) { }
        protected void EndCoyoteTime()
        {
            m_coyoteCountdown = 0;
        }
        #endregion

        #region State
        public void SetMovementState(MotorMovementState newState)
        {
            if (m_movementState != newState)
            {
                m_movementState = newState;

                OnMovementStateChanged();
            }
        }
        protected virtual void OnMovementStateChanged()
        {
            switch (m_movementState)
            {
                case MotorMovementState.Velocity:
                    m_rb.isKinematic = false;
                    break;
                case MotorMovementState.Kinematic:
                    m_rb.isKinematic = true;
                    break;
                case MotorMovementState.Navigation:
                    m_rb.isKinematic = true;
                    break;
                default:
                    break;
            }
        }

        public void OnActionStart()
        {
            SetMovementState(MotorMovementState.Action);
            OnActionStartInternal();
        }
        protected virtual void OnActionStartInternal() { }

        public void OnActionEnd()
        {
            OnActionEndInternal();
        }
        protected virtual void OnActionEndInternal()
        {
            SetMovementState(m_defaultMovementState);
        }
        #endregion

#if UNITY_EDITOR
        #region Debug
        private void OnDrawGizmos()
        {
            Matrix4x4 orig = Handles.matrix;

            Handles.color = Color.blue;
            Handles.DrawWireCube(m_rb.position, new Vector3(m_groundCheckRadius, 0.1f, m_groundCheckRadius));
            Handles.DrawWireCube(m_rb.position + (Vector3.down * m_groundCheckDistance), new Vector3(m_groundCheckRadius, 0.1f, m_groundCheckRadius));

            Handles.color = Color.green;
            m_feetOffset = new(0f, -m_col.bounds.extents.y, 0f);
            Handles.DrawWireCube(FeetPosition, Vector3.one / 5f);

            Handles.matrix = transform.localToWorldMatrix;
            Handles.matrix = orig;
        }
        #endregion
    }
#endif
}
