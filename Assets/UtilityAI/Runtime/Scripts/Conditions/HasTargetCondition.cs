using UnityEngine;

namespace Stirge.UtilityAI.Conditions
{
    using Combat;
    using Serialization;

    public class HasTargetCondition : Condition<IScorable>, INotSetupable
    {
        public override bool Evaluate(CombatEntity user, CombatEntity target)
        {
            if (user is UtilityEnemy enemy)
            {
                return enemy.HasTarget;
            }
            return false;
        }
    }
}
