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
        [SerializeField] private CombatEntity m_target;

        private UtilityBrain m_brain;
        public UtilityBrain Brain => m_brain;

        // properties
        public UtilityEnemyMotor EnemyMotor => (UtilityEnemyMotor)Motor;
        public new Transform transform => Motor.transform;
        public Rigidbody Rigidbody => Motor.Rigidbody;
        public NavMeshAgent NavMeshAgent => EnemyMotor.NavMeshAgent;
        public CombatEntity Target => m_target;

        private void Start()
        {
            //Time.fixedDeltaTime = 0.333f;

            m_brain = m_serializedBrain.CreateRuntimeBrain();
            m_brain.Start();
        }

        private void Update()
        {
            m_brain.Update(this, m_target);
        }

        #region Transformation
        public override Vector3 GetPosition()
        {
            return Motor.transform.position;
        }
        public override void SetPosition(Vector3 newPosition)
        {
            Motor.SetPosition(newPosition);
        }
        public override Quaternion GetRotation()
        {
            return Motor.transform.rotation;
        }
        public override void SetRotation(Quaternion newRotation)
        {
            Motor.SetRotation(newRotation);
        }
        public override void SetRotation(Vector3 eulerRotation)
        {
            Motor.SetRotation(Quaternion.Euler(eulerRotation));
        }
        public override Vector3 GetForward()
        {
            return Motor.transform.forward;
        }
        #endregion

        #region Physics
        public override void MovePosition(Vector3 newPosition)
        {
            Motor.SetPosition(newPosition);
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
