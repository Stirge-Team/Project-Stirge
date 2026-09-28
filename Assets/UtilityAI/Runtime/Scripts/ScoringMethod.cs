using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    public abstract class ScoringMethod<TScorable> : IScoringMethod<TScorable> where TScorable : IScorable
    {
        private TScorable m_scorable;
        protected float m_scoreScaling;

        float IScoringMethod<TScorable>.scoreScaling { set => m_scoreScaling = value; }
        TScorable IScoringMethod<TScorable>.scorable { set => m_scorable = value; }
        
        protected TScorable Scorable => m_scorable;

        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float score = EvaluateInternal(user, target);
            return score * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);
    }
}
