using UnityEngine;

namespace Stirge.UtilityAI.ScoringMethods
{
    using Combat;
    using Serialization;

    public class ConstantScoringMethod : ScoringMethod, ISetupable<float>
    {
        private float m_score;

        public void Setup(float score)
        {
            m_score = score;
        }

        protected override float EvaluateInternal(CombatEntity user, CombatEntity target)
        {
            return m_score;
        }
    }
}
