using UnityEngine;

namespace Stirge.UtilityAI.ScoringMethods
{
    using Combat;
    using Serialization;

    public class DamageScoringMethod : ScoringMethod, INotSetupable
    {
        protected override float EvaluateInternal(CombatEntity user, CombatEntity target)
        {
            return m_action.damage;
        }
    }
}
