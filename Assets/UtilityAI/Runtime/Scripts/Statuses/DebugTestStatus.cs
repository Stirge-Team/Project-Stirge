using Stirge.Combat;
using Stirge.Serialization;
using System;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    public class DebugTestStatus : Status, INotSetupable
    {
        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return 1f;
        }

        public override Type StatusType => typeof(DebugTestStatus);

        protected override void OnApplyInternal(CombatEntity inflicter, CombatEntity target)
        {
            Debug.Log(m_displayName + " applied.");
        }
        protected override void UpdateInternal(CombatEntity target)
        {
            Debug.Log(m_displayName + " updated.");
        }
        protected override void OnClearInternal(CombatEntity target)
        {
            Debug.Log(m_displayName + " cleared.");
        }
    }
}
