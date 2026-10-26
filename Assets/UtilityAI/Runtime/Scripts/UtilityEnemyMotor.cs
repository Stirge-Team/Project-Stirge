using UnityEngine;
using UnityEngine.AI;

namespace Stirge.UtilityAI
{
    using Combat;

    public class UtilityEnemyMotor : CombatEntityMotor
    {
        [Header("Enemy Components")]
        [SerializeField] private UtilityEnemy m_enemy;
        [SerializeField] private NavMeshAgent m_nav;

        // navigation
        private bool m_headingIsTargetPosition;

        // properties
        public NavMeshAgent NavMeshAgent => m_nav;
        public bool HeadingIsTargetPosition => m_headingIsTargetPosition;

        #region Unity Events
        protected override void OnStart()
        {
            if (MovementState != MotorMovementState.Navigation)
                m_nav.enabled = false;
        }
        #endregion

        #region Transformation
        protected override void OnMovePosition(Vector3 newPosition)
        {
            if (MovementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentPosition();
            }
        }
        protected override void OnSetPosition(Vector3 newPosition)
        {
            if (MovementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentPosition();
            }
        }
        protected override void OnSetRotation(Quaternion newRotation)
        {
            if (MovementState == MotorMovementState.Navigation)
            {
                SyncNavMeshAgentRotation();
            }
        }
        protected override void OnSetPositionAndRotation(Vector3 newPosition, Quaternion newRotation)
        {
            if (MovementState == MotorMovementState.Navigation)
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
                m_targetHeading = (m_enemy.Target.Motor.GetPosition() - Rigidbody.position).normalized;
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

        protected override void OnBecomeGrounded()
        {
            if (!m_enemy.IsPerformingAction)
                SetMovementState(MotorMovementState.Navigation);
        }
        protected override void OnBecomeNotGrounded()
        {
            if (!m_enemy.IsPerformingAction)
                SetMovementState(MotorMovementState.Velocity);
        }
        #endregion

        #region Navigation
        protected override void UpdateNavigation()
        {
            // If the Agent is not on the NavMesh, keep trying until it is
            if (!m_nav.isOnNavMesh)
            {
                SyncNavMeshAgentPosition();
                return;
            }
            
            // Move Rigidbody to follow NavMeshAgent's calculated path
            Vector3 nextPosition = m_nav.nextPosition;
            Rigidbody.MovePosition(nextPosition);

            // Optionally, update rotation manually
            if (m_nav.hasPath)
            {
                Vector3 dir = m_nav.desiredVelocity;
                dir.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                Rigidbody.MoveRotation(targetRotation);
            }
        }

        private void SyncNavMeshAgentPosition()
        {
            // Check if NavAgent actually needs to be moved
            if (m_nav.transform.position != Rigidbody.position)
            {
                // Sync to position on NavMesh
                if (NavMesh.SamplePosition(Rigidbody.position, out NavMeshHit hit, 4f, NavMesh.AllAreas))
                {
                    m_nav.Warp(hit.position);
                    if (!m_nav.isOnNavMesh)
                    {
                        // restart the component as this can help with placing it on the NavMesh for some reason, Unity shit
                        m_nav.enabled = false;
                        m_nav.enabled = true;
                    }

                    // NavMeshAgent.Warp() clears destination, so reset it if there's a target
                    if (m_enemy.HasTarget)
                    {
                        m_nav.SetDestination(m_enemy.TargetPosition);
                    }
                }
                else
                {
                    Debug.LogError($"Could not place Agent on NavMesh at position {Rigidbody.position}", NavMeshAgent);
                }
            }
        }
        private void SyncNavMeshAgentRotation()
        {
            m_nav.transform.rotation = Rigidbody.rotation; // does this work?
        }
        #endregion

        #region State
        protected override void OnMovementStateChangedToVelocity()
        {
            Rigidbody.isKinematic = false;
            m_nav.enabled = false;
        }
        protected override void OnMovementStateChangedToKinematic()
        {
            Rigidbody.isKinematic = true;
            m_nav.enabled = false;
        }
        protected override void OnMovementStateChangedToNavigation()
        {
            Rigidbody.isKinematic = true;
            m_nav.enabled = true;
            SyncNavMeshAgentPosition();
            // no need to Set Destination as this happens in Sync
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
