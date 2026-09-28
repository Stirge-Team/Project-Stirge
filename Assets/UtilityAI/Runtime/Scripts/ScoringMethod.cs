using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    public abstract class ScoringMethod<T> : IScoringMethod<T> where T : IScorable
    {
        private T m_scorable;
        protected float m_scoreScaling;

        float IScoringMethod<T>.scoreScaling { set => m_scoreScaling = value; }
        
        protected T Scorable => m_scorable;

        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float score = EvaluateInternal(user, target);
            return score * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);
    }
}
