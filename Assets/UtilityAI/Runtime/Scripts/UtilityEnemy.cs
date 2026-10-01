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

        private CombatEntity m_target;
        private Vector3 m_targetPosition;
        private bool m_hasTarget = false;

        private UtilityBrain m_brain;
        private UtilityEnemyMotor m_enemyMotor;

        // component properties
        public UtilityBrain Brain => m_brain;
        public UtilityEnemyMotor EnemyMotor => m_enemyMotor;
        public new Transform transform => Motor.transform;
        public Rigidbody Rigidbody => Motor.Rigidbody;
        public NavMeshAgent NavMeshAgent => EnemyMotor.NavMeshAgent;

        // field properties
        public CombatEntity Target => m_target;
        public Vector3 TargetPosition => m_targetPosition;
        public bool HasTarget => m_hasTarget;
        public float DistanceToTarget => m_hasTarget ? Vector3.Distance(Motor.GetPosition(), m_targetPosition) : float.PositiveInfinity;

        #region Unity Events
        private void Awake()
        {
            m_enemyMotor = (UtilityEnemyMotor)Motor;
        }

        private void Start()
        {
            //Time.fixedDeltaTime = 0.333f;

            m_hasTarget = false;

            m_brain = m_serializedBrain.CreateRuntimeBrain();
            m_brain.Start();
        }

        private void Update()
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

        public bool IsNotWithinStoppingDistanceOf(Vector3 position)
        {
            return Vector3.SqrMagnitude(position - Motor.GetPosition()) > NavMeshAgent.stoppingDistance * NavMeshAgent.stoppingDistance;
        }
        #endregion
    }
}
