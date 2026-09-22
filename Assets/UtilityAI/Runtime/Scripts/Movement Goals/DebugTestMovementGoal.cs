using Stirge.Combat;
using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI.MovementGoals
{
    public class DebugTestMovementGoal : MovementGoal, ISetupable<float, string>
    {
        private float m_score;
        private string m_message;
        private float m_timer;

        public void Setup(float score, string message)
        {
            m_score = score;
            m_message = message;
        }

        public override void Perform(UtilityEnemy user, CombatEntity target)
        {
            m_timer += Time.deltaTime;
            Debug.Log(m_message + " : " + m_timer);
        }

        public override void Reset()
        {
            m_timer = 0;
        }

        protected override float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return m_score;
        }
    }
}
