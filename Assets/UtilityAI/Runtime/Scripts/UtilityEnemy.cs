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
        public override void InflictStatus(Status status, CombatEntity user)
        {
            // NOTE: This needs to be changed. Statuses should be able to support:
            // - Statuses with no stacking. Adding a new identical Status will add another
            // - Statuses with no stacking. Cannot add identical Status while one already is inflicted.
            // - Statuses with stacking. Adding a new identical Status will add to its current stacks, changing its Resolve effect.
            // Timothy Cain video on Status Effects: https://www.youtube.com/watch?v=SH35RmM1BFM&t=9s
            // if inflicted with Statuses of same type, get references
            
            Type statusType = status.GetType();
            int indexOfExistingStatus = GetIndexOfStatus(statusType);
            // if matching status exists
            if (indexOfExistingStatus != -1)
            {
                switch (status.stackType)
                {
                    case StatusStackType.Stackable:
                        Status existingStackableStatus = m_inflictedStatuses[indexOfExistingStatus];
                        int existingStacks = existingStackableStatus.currentStackCount;
                        int maxStacks = existingStackableStatus.maxStacks;
                        if (existingStacks < maxStacks)
                        {
                            // add new stacks
                            existingStackableStatus.currentStackCount = Mathf.Min(existingStacks + status.currentStackCount, maxStacks);
                        }
                        break;
                    case StatusStackType.Unstackable:
                        // fall through to standard Add and Apply
                        break;
                    case StatusStackType.Unique:
                        // Exit to avoid adding multiples of Unique Status
                        return;
                    default:
                        return;
                }
            }
            m_inflictedStatuses.Add(status);
            status.OnApply(user, this);
        }

        /// <summary>
        /// Returns -1 if no <see cref="Status"/> of type <paramref name="statusType"/> was found.
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>Index of first inflicted <see cref="Status"/> with matching type.</returns>
        public int GetIndexOfStatus(Type statusType)
        {
            return m_inflictedStatuses.FindIndex(status => status.GetType() == statusType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>The number of the provided <paramref name="statusType"/> the Enemy is inflicted with.</returns>
        public int GetNumberOfInflictedStatus(Type statusType)
        {
            return m_inflictedStatuses.FindAll(status => status.statusType == statusType).Count;
        }
        #endregion
    }
}
