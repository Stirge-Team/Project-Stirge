using UnityEngine;

namespace Stirge.UtilityAI.ScoringMethods
{
    using Combat;
    using Serialization;

    public class TrigScoringMethod : ScoringMethod<IScorable>, ISetupable<bool, float, float, float, float>
    {
        private bool m_useCosine;
        private float m_a;
        private float m_b;
        private float m_c;
        private float m_d;

        public void Setup(bool useCosine, float a, float b, float c, float d)
        {
            m_useCosine = useCosine;
            m_a = a;
            m_b = b;
            m_c = c;
            m_d = d;
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            if (m_useCosine)
            {
                return m_a * Mathf.Cos(m_b * (Time.time - m_c)) + m_d;
            }
            else
            {
                return m_a * Mathf.Sin(m_b * (Time.time - m_c)) + m_d;
            }
        }
    }
}
