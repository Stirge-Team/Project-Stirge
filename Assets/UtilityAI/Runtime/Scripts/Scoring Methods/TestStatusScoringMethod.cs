using UnityEngine;

namespace Stirge.UtilityAI.ScoringMethods
{
    using Combat;
    using Serialization;

    public class TestStatusScoringMethod : ScoringMethod<Status>, INotSetupable
    {
        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return 1f;
        }
    }
}
