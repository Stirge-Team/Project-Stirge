using UnityEngine;
using UnityEngine.AI;

namespace Stirge.UtilityAI
{
    using Combat;

    [RequireComponent(typeof(Rigidbody))]
    public class UtilityEnemyMotor : CombatEntityMotor
    {
        [Header("Enemy Components")]
        [SerializeField] private UtilityEnemy m_enemy;
        [SerializeField] private NavMeshAgent m_nav;

        [Header("Navigation Fields")]
        [SerializeField, Min(0)] private float m_navStoppingDistance;

        // navigation
        private bool m_headingIsTargetPosition;

        // properties
        public NavMeshAgent NavMeshAgent => m_nav;
        public bool HeadingIsTargetPosition => m_headingIsTargetPosition;

        #region Unity Events
        protected override void OnStart()
        {
            m_nav.enabled = false;

            m_nav.autoBraking = false;
            m_nav.autoRepath = false;
            m_nav.updatePosition = false;
            m_nav.updateRotation = false;
            m_nav.speed = groundedMovementProperties.HorizontalTopSpeed;
            m_nav.acceleration = groundedMovementProperties.Acceleration;
            m_nav.stoppingDistance = m_navStoppingDistance;
            m_nav.angularSpeed = groundedMovementProperties.AngularSpeed;
        }
        #endregion

        #region Transformation
        protected override void OnSetPosition(Vector3 newPosition)
        {
            if (movementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentPosition();
            }
        }
        protected override void OnSetRotation()
        {
            if (movementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentRotation();
            }
        }
        protected override void OnSetPositionAndRotation(Vector3 newPosition, Quaternion newRotation)
        {
            if (movementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentPosition();
                SyncNavMeshAgentRotation();
            }
        }
        #endregion

        #region Physics
        public void ChangeHeadingBehaviour(bool lookAtTarget)
        {
            m_headingIsTargetPosition = lookAtTarget;
        }
        protected override void OnPreUpdateHeading()
        {
            // if should update heading to match movement direction
            // determine what target heading is
            if (m_headingIsTargetPosition)
            {
                // determine what target heading is
                m_targetHeading = (m_enemy.Target.GetPosition() - Rigidbody.position).normalized;
                m_targetHeading.y = 0;
            }
            else
            {
                m_targetHeading = m_currentVelocity.normalized;
            }
        }

        protected override void OnChangeGroundedMovementProperties(MovementProperties newProperties)
        {
            m_nav.speed = newProperties.HorizontalTopSpeed;
            m_nav.acceleration = newProperties.Acceleration;
            m_nav.angularSpeed = newProperties.AngularSpeed;
        }
        #endregion

        #region Navigation
        protected override void UpdateNavigation()
        {
            // if has no path, change state
            if (!m_nav.hasPath)
            {
                SetMovementState(MotorMovementState.Velocity);
                return;
            }

            // Move Rigidbody to follow NavMeshAgent's calculated path
            Vector3 nextPosition = m_nav.nextPosition;
            Rigidbody.MovePosition(nextPosition);

            // Optionally, update rotation manually
            if (m_nav.hasPath)
            {
                Quaternion targetRotation = Quaternion.LookRotation(m_nav.desiredVelocity);
                Rigidbody.MoveRotation(targetRotation);
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
            if (m_nav.hasPath)
                m_nav.isStopped = true;
        }

        private void SyncNavMeshAgentPosition()
        {
            // TODO: Check if NavAgent actually needs to be moved
            // Also, I don't think this works
            // Could try using the grounded RaycastHit buffer to get a position

            Vector3 currentDestination = m_nav.destination;

            // determine position to sync to, prefer agent is on spot on NavMesh
            Vector3 syncPosition;
            if (NavMesh.SamplePosition(Rigidbody.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                syncPosition = hit.position;
            else
                syncPosition = Rigidbody.position;

            m_nav.Warp(syncPosition);

            // if agent is active, then preserve destination after the warp
            if (movementState == MotorMovementState.Navigation)
                m_nav.SetDestination(currentDestination);
        }
        private void SyncNavMeshAgentRotation()
        {
            m_nav.transform.rotation = Rigidbody.rotation; // does this work?
        }
        #endregion

        #region State
        protected override void OnMovementStateChanged()
        {
            switch (movementState)
            {
                case MotorMovementState.Velocity:
                    Rigidbody.isKinematic = false;
                    m_nav.enabled = false;
                    break;
                case MotorMovementState.Kinematic:
                    Rigidbody.isKinematic = true;
                    m_nav.enabled = false;
                    break;
                case MotorMovementState.Navigation:
                    Rigidbody.isKinematic = true;
                    m_nav.enabled = true;
                    SyncNavMeshAgentPosition();
                    break;
                default:
                    break;
            }
        }

        protected override void OnActionEndInternal()
        {
            if (IsGrounded)
                SetMovementState(MotorMovementState.Navigation);
            else
                SetMovementState(MotorMovementState.Velocity);
        }
        #endregion


        [ContextMenu("add random force")]
        public void AddRandomForce()
        {
            AddForce(3f * new Vector3((Random.value * 2f - 1f) * 3f, Random.value * 3f, (Random.value * 2f - 1f) * 3f));
        }
    }

}
