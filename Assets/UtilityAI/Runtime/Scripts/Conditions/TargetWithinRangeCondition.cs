using UnityEngine;

namespace Stirge.UtilityAI.Conditions
{
    using Combat;
    using Serialization;

    public class TargetWithinRangeCondition : Condition<IScorable>, INotSetupable
    {
        public override bool Evaluate(CombatEntity user, CombatEntity target)
        {
            return false;
        }
    }
}
