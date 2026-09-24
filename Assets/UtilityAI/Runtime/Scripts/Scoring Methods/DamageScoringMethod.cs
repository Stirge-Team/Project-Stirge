using UnityEngine;

namespace Stirge.UtilityAI.ScoringMethods
{
    using Combat;
    using Serialization;

    public class DamageScoringMethod : ScoringMethod, INotSetupable
    {
        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return user.Brain.CurrentAction.damage;
        }
    }
}
