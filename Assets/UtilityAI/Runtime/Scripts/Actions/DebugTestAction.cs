using UnityEngine;

namespace Stirge.UtilityAI.Actions
{
    using Combat;
    using Serialization;

    public class DebugTestAction : Action, ISetupable<float, string, bool>
    {
        private float m_score;
        private string m_message;
        private bool m_useCosineInsteadOfSine;

        public void Setup(float score, string message, bool useCosineInsteadOfSine)
        {
            m_score = score;
            m_message = message;
            m_useCosineInsteadOfSine = useCosineInsteadOfSine;
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            if (m_useCosineInsteadOfSine)
                return m_score * Mathf.Cos(Time.time);
            else
                return m_score * Mathf.Sin(Time.time);
        }

        public override void Perform(CombatEntity user, CombatEntity target)
        {
            Debug.Log(m_message);
        }
    }
}
