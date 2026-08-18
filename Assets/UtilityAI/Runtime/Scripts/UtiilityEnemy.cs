using Stirge.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public class UtiilityEnemy : CombatEntity
    {
        [SerializeField] private SerializedAction[] m_serializedActions;
        [SerializeField] private CombatEntity m_target;

        private Action[] m_actions;

        // Stats
        private float m_baseDamage = 1f;

        private void Start()
        {
            Time.fixedDeltaTime = 0.333f;

            int count = m_serializedActions.Length;
            m_actions = new Action[count];
            for (int i = 0; i < count; i++)
            {
                m_actions[i] = m_serializedActions[i].CreateRuntimeAction();
            }
        }

        private void FixedUpdate()
        {
            foreach (var action in m_actions)
            {
                Debug.Log(action.Evaluate(this, m_target));
            }
        }

        public override void InflictStatus(Status status, CombatEntity user)
        {
            // NOTE: This needs to be changed. Statuses should be able to support:
            // - Statuses with no stacking. Adding a new identical Status will add another
            // - Statuses with no stacking. Cannot add identical Status while one already is inflicted.
            // - Statuses with stacking. Adding a new identical Status will add to its current stacks, changing its Resolve effect.
            // Timothy Cain video on Status Effects: https://www.youtube.com/watch?v=SH35RmM1BFM&t=9s
            // if inflicted with Statuses of same type, get references
            
            Type statusType = status.statusType;
            int indexOfExistingStatus = GetIndexOfStatus(statusType);
            // if matching status exists
            if (indexOfExistingStatus != -1)
            {
                switch (status.data.stackType)
                {
                    case StatusStackType.Stackable:
                        Status existingStackableStatus = m_inflictedStatuses[indexOfExistingStatus];
                        int existingStacks = existingStackableStatus.currentStackCount;
                        int maxStacks = existingStackableStatus.data.maxStacks;
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
            status.Apply(user, this);
        }

        /// <summary>
        /// Returns -1 if no <see cref="Status"/> of type <paramref name="statusType"/> was found.
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>Index of first inflicted <see cref="Status"/> with matching type.</returns>
        public int GetIndexOfStatus(Type statusType)
        {
            return m_inflictedStatuses.FindIndex(status => status.statusType == statusType);
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
    }
}
