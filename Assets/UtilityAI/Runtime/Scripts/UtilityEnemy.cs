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

        #region Status
        public override void InflictStatus(Status newStatus, CombatEntity user)
        {            
            Type statusType = newStatus.StatusType;
            int indexOfExistingStatus = GetIndexOfStatus(statusType);

            // If a Status of the same type already exists
            if (indexOfExistingStatus != -1)
            {
                switch (newStatus.StackType)
                {
                    case StatusStackType.Stackable:
                        Status existingStackableStatus = m_inflictedStatuses[indexOfExistingStatus];
                        existingStackableStatus.AddStacks(newStatus.CurrentStacks);
                        return; // don't add again
                    case StatusStackType.Unique:
                        return; // don't add again
                    default:
                        break;
                }
            }

            m_inflictedStatuses.Add(newStatus);
            newStatus.OnApply(user, this);
        }

        /// <summary>
        /// Returns -1 if no <see cref="Status"/> of type <paramref name="statusType"/> was found.
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>Index of first inflicted <see cref="Status"/> with matching type.</returns>
        public int GetIndexOfStatus(Type statusType)
        {
            return m_inflictedStatuses.FindIndex(status => status.StatusType == statusType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>The number of the provided <paramref name="statusType"/> the Enemy is inflicted with.</returns>
        public int GetNumberOfInflictedStatus(Type statusType)
        {
            return m_inflictedStatuses.FindAll(status => status.StatusType == statusType).Count;
        }
        #endregion
    }
}
