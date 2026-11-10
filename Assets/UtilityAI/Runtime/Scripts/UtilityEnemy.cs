using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using UnityEngine.AI;

    public class UtilityEnemy : CombatEntity
    {
        [Header("Utility Properties")]
        [SerializeField] private SerializedUtilityBrain m_serializedBrain;
        private UtilityEnemyMotor m_enemyMotor;

        private CombatEntity m_target;
        private Vector3 m_targetPosition;
        private bool m_hasTarget = false;

        private UtilityBrain m_brain;

        public event Action<UtilityEnemy> onDeath;

        // component properties
        public UtilityEnemyMotor EnemyMotor => m_enemyMotor;
        public new Transform transform => Motor.transform;
        public Rigidbody Rigidbody => Motor.Rigidbody;
        public NavMeshAgent NavMeshAgent => m_enemyMotor.NavMeshAgent;

        // field properties
        public CombatEntity Target => m_target;
        public Vector3 TargetPosition => m_targetPosition;
        public bool HasTarget => m_hasTarget;
        public float DistanceToTarget => m_hasTarget ? Vector3.Distance(Motor.GetPosition(), m_targetPosition) : float.PositiveInfinity;

        #region Unity Events
        protected override void AwakeThis()
        {
            m_enemyMotor = (UtilityEnemyMotor)Motor;
        }

        protected override void StartThis()
        {
            //Time.fixedDeltaTime = 0.333f;

            m_hasTarget = false;

            m_brain = m_serializedBrain.CreateRuntimeBrain();
            m_brain.Start();
        }

        protected override void UpdateThis()
        {
            if (m_target != null)
                m_targetPosition = m_target.Motor.GetPosition();
            
            m_brain.Update(this, m_target);
        }
        #endregion

        #region Navigation
        public void SetTarget(CombatEntity newTarget)
        {
            m_target = newTarget;
            m_hasTarget = true;
        }
        public void SetTargetPosition(Vector3 newTargetPosition)
        {
            m_targetPosition = newTargetPosition;
            m_target = null;
            m_hasTarget = true;
        }
        public void ClearTarget()
        {
            m_target = null;
            m_hasTarget = false;
        }
        #endregion

        #region DeathState
        protected override void OnDeath()
        {
            onDeath.Invoke(this);
            Destroy(transform.parent.gameObject);
        }
        #endregion

        #region Status
        protected override void OnStartStun(float duration)
        {
            if (Motor.MovementState == MotorMovementState.Navigation)
            {
                EnemyMotor.NavMeshAgent.isStopped = true;
            }
        }
        protected override void OnStopStun()
        {
            if (EnemyMotor.NavMeshAgent.isStopped)
                EnemyMotor.NavMeshAgent.isStopped = false;
        }
        #endregion

#if UNITY_EDITOR
        public bool TryGetBrainDebugInfo(out BrainDebugInfo info)
        {
            if (m_brain != null)
            {
                info = new();
                m_brain.GetBrainDebugInfo(ref info);
                return true;
            }
            info = null;
            return false;
        }

        private void OnDrawGizmos()
        {
            m_brain?.OnDrawGizmos(this, m_target);
        }
#endif
    }
}
