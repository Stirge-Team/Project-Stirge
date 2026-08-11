using UnityEngine;
using UnityEngine.AI;

namespace Stirge.Enemy
{
    public enum MotorMovementState
    {
        Velocity,
        Kinematic,
        Navigation
    }

    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform m_transform;
        [SerializeField] private Enemy m_enemy;
        
        [Header("Components")]
        [SerializeField] private Rigidbody m_rb;
        [SerializeField] private CapsuleCollider m_collider;
        [SerializeField] private NavMeshAgent m_nav;

        [Header("Movement Fields")]
        [SerializeField] private float m_topSpeed;
        [SerializeField] private float m_acceleration;
        [SerializeField] private LayerMask m_walkableLayers;
        [SerializeField] private float m_groundCheckDistance;
        [SerializeField] private float m_groundCheckRadius;

        [Header("Navigation Fields")]
        [SerializeField, Min(0)] private float m_navStoppingDistance;
        [SerializeField, Min(0)] private float m_angularSpeed;
        [SerializeField, Range(0f, 90f)] private float m_slopeLimit;

        // velocities
        private Vector3 m_currentVelocity;
        private Vector3 m_attackVelocity;

        // heading
        private Vector3 m_targetHeading;
        private bool m_headingIsTargetPosition;

        // movement state
        private MotorMovementState m_movementState = MotorMovementState.Kinematic;
        private float m_airTime;
        private bool m_isGrounded;
        private readonly RaycastHit[] m_groundedCheckHits = new RaycastHit[10];

        // properties
        public new Transform transform => m_transform;
        public float topSpeed => m_topSpeed;
        public float angularSpeed => m_angularSpeed;
        public bool headingIsTargetPosition => m_headingIsTargetPosition;

        // runtime properties
        public Vector3 currentVelocity => m_currentVelocity;
        public Vector3 feetPosition => m_rb.position + m_collider.center - Vector3.down * m_collider.height / 2f;

        #region Unity Events
        private void Start()
        {
            m_rb.isKinematic = false;
            m_nav.enabled = false;

            m_rb.useGravity = false;

            m_nav.autoBraking = false;
            m_nav.autoRepath = false;
            m_nav.updatePosition = false;
            m_nav.updateRotation = false;
            m_nav.speed = m_topSpeed;
            m_nav.acceleration = m_acceleration;
            m_nav.stoppingDistance = m_navStoppingDistance;
            m_nav.angularSpeed = m_angularSpeed;
            
            if (Application.isPlaying)
                SetMovementState(MotorMovementState.Velocity);
        }

        private void Update()
        {
            if (!m_isGrounded)
            {
                m_airTime += Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            switch (m_movementState)
            {
                case MotorMovementState.Velocity:
                    UpdateVelocity();
                    UpdateHeading();
                    break;
                case MotorMovementState.Kinematic:
                    break;
                case MotorMovementState.Navigation:
                    UpdateNavigation();
                    break;
            }

            Debug.Log($"Attack Velocity: {m_attackVelocity}. Rigidbody Linear Velocity: {m_rb.linearVelocity}.");
        }
        #endregion

        #region Transformation
        public void SetPosition(Vector3 newPosition)
        {
            m_rb.MovePosition(newPosition);
            if (m_movementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentPosition();
            }
        }
        public void SetRotation(Quaternion newRotation)
        {
            m_rb.MoveRotation(newRotation);
            if (m_movementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentRotation();
            }
        }
        public void SetPositionAndRotation(Vector3 newPosition, Quaternion newRotation)
        {
            m_rb.Move(newPosition, newRotation);
            if (m_movementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentPosition();
                SyncNavMeshAgentRotation();
            }
        }
        #endregion

        #region Physics
        public void SetAttackVelocity(Vector3 newAttackVelocity)
        {
            SetMovementState(MotorMovementState.Velocity);
            m_attackVelocity = newAttackVelocity;
        }

        public bool PerformIsGroundedCheck()
        {
            if (!m_collider)
                return false;

            // check the area under the Enemy to check for objects on Layers marked in m_walkableLayers
            int hitCount = Physics.BoxCastNonAlloc(m_rb.position + m_collider.center,
                new Vector3(m_groundCheckRadius, m_groundCheckDistance, m_groundCheckRadius),
                Vector3.down, m_groundedCheckHits, transform.rotation, m_collider.height / 2f,
                m_walkableLayers, QueryTriggerInteraction.Ignore);

            // if no hits, not grounded
            if (hitCount == 0)
            {
                return false;
            }
            else
            {
                // if changing from not grounded to grounded aka landing
                if (!m_isGrounded)
                {
                    m_airTime = 0f;
                    //m_currentVelocity.y = 0;
                }

                // get closest hit walkable object
                RaycastHit closestHit = m_groundedCheckHits[0];
                if (hitCount > 1)
                {
                    float closestDistance = Vector3.Distance(closestHit.point, feetPosition);
                    for (int i = 1; i < hitCount; i++)
                    {
                        float distance = Vector3.Distance(m_groundedCheckHits[i].point, feetPosition);
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

        private void UpdateVelocity()
        {
            // only do cast once per physics frame
            m_isGrounded = PerformIsGroundedCheck();

            // if has attack velocity, resolve
            if (m_attackVelocity != Vector3.zero)
            {
                m_rb.linearVelocity = m_attackVelocity;
                return;
            }

            if (!m_isGrounded)
            {
                m_currentVelocity += Physics.gravity * Time.fixedDeltaTime;
            }

            m_rb.linearVelocity = m_currentVelocity;
        }
        private void UpdateHeading()
        {
            // if should update heading to match movement direction
            // determine what target heading is
            if (m_headingIsTargetPosition)
            {
                // determine what target heading is
                m_targetHeading = (m_enemy.TargetTransform.position - m_rb.position).normalized;
                m_targetHeading.y = 0;
            }
            else
            {
                m_targetHeading = m_attackVelocity != Vector3.zero ? m_attackVelocity.normalized : m_currentVelocity.normalized;
            }

            // if heading does not already match movement direction
            if (transform.forward != m_targetHeading)
            {
                Vector3 currentHeading = Vector3.RotateTowards(transform.forward, m_targetHeading, m_angularSpeed * Time.fixedDeltaTime, 0f);
                Quaternion newRotation = Quaternion.LookRotation(currentHeading);
                m_rb.MoveRotation(newRotation);
            }
        }

        public void ResetVelocity()
        {
            m_currentVelocity = Vector3.zero;
        }
        public void ResetAttackVelocity()
        {
            m_attackVelocity = Vector3.zero;
        }

        public void AddForce(Vector3 force)
        {
            SetMovementState(MotorMovementState.Velocity);
            m_currentVelocity += force / m_rb.mass;
        }
        public void AddVelocity(Vector3 velocity)
        {
            SetMovementState(MotorMovementState.Velocity);
            m_currentVelocity += velocity;
        }

        public void SetTopSpeed(float newTopSpeed)
        {
            m_topSpeed = newTopSpeed;
            m_nav.speed = newTopSpeed;
        }
        public void SetAngularSpeed(float newAngularSpeed)
        {
            m_angularSpeed = newAngularSpeed;
            m_nav.angularSpeed = newAngularSpeed;
        }
        public void SetAcceleration(float newAcceleration)
        {
            m_acceleration = newAcceleration;
            m_nav.acceleration = newAcceleration;
        }

        public void ChangeHeadingBehaviour(bool lookAtTarget)
        {
            m_headingIsTargetPosition = lookAtTarget;
        }
        #endregion

        #region Navigation
        private void UpdateNavigation()
        {
            // if has no path, change state
            if (!m_nav.hasPath)
            {
                SetMovementState(MotorMovementState.Velocity);
                return;
            }
            
            // Move Rigidbody to follow NavMeshAgent's calculated path
            Vector3 nextPosition = m_nav.nextPosition;
            m_rb.MovePosition(nextPosition);

            // Optionally, update rotation manually
            if (m_nav.hasPath)
            {
                Quaternion targetRotation = Quaternion.LookRotation(m_nav.desiredVelocity);
                m_rb.MoveRotation(targetRotation);
            }
        }

        public bool SetDestination(Vector3 targetPosition)
        {
            SetMovementState(MotorMovementState.Navigation);
            bool pathFound = m_nav.SetDestination(targetPosition);
            return pathFound;
        }
        public void ClearDestination()
        {
            m_nav.path = null;
        }

        private void SyncNavMeshAgentPosition()
        {
            Vector3 currentDestination = m_nav.destination;

            // determine position to sync to, prefer agent is on spot on NavMesh
            Vector3 syncPosition;
            if (NavMesh.SamplePosition(m_rb.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                syncPosition = hit.position;
            else
                syncPosition = m_rb.position;

            m_nav.Warp(syncPosition);

            // if agent is active, then preserve destination after the warp
            if (m_movementState == MotorMovementState.Navigation)
                m_nav.SetDestination(currentDestination);
        }
        private void SyncNavMeshAgentRotation()
        {
            m_nav.transform.rotation = m_rb.rotation; // does this work?
        }
        #endregion

        #region State
        public void SetMovementState(MotorMovementState newState)
        {
            if (m_movementState != newState)
            {
                m_movementState = newState;

                switch (newState)
                {
                    case MotorMovementState.Velocity:
                        m_rb.isKinematic = false;
                        m_nav.enabled = false;
                        break;
                    case MotorMovementState.Kinematic:
                        m_rb.isKinematic = true;
                        m_nav.enabled = false;
                        break;
                    case MotorMovementState.Navigation:
                        m_rb.isKinematic = true;
                        m_nav.enabled = true;
                        SyncNavMeshAgentPosition();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
