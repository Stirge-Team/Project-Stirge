using System;
using UnityEngine;

namespace Stirge.UtilityAI.Statuses
{
    using Combat;
    using Serialization;

    public class DebugTestStatus : Status, INotSetupable
    {
        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return 1f;
        }

        public override Type StatusType => typeof(DebugTestStatus);

        protected override void OnApplyInternal(CombatEntity inflicter, CombatEntity target)
        {
            Debug.Log(DisplayName + " applied.");
        }
        protected override void UpdateInternal(CombatEntity target)
        {
            Debug.Log(DisplayName + " updated.");
        }
        protected override void OnClearInternal(CombatEntity target)
        {
            Debug.Log(DisplayName + " cleared.");
        }
    }
}
