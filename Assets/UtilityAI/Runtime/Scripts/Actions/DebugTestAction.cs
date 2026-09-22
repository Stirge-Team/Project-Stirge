using UnityEngine;

namespace Stirge.UtilityAI.Actions
{
    using Combat;
    using Serialization;

    public class DebugTestAction : Action, ISetupable<float, string>
    {
        private float m_score;
        private string m_message;

        public void Setup(float score, string message)
        {
            m_score = score;
            m_message = message;
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return m_score;
        }

        public override void Perform(CombatEntity user, CombatEntity target)
        {
            Debug.Log(m_message);
        }
    }
}
