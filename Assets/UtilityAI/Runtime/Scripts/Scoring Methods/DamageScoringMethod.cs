using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;

    public class DamageScoringMethod : ScoringMethod, ISetupable<Action>
    {
        private Action m_action;
        
        public void Setup(Action action)
        {
            m_action = action;
        }

        protected override float EvaluateInternal(CombatEntity user, CombatEntity target)
        {
            return m_action.damage;
        }
    }
}
