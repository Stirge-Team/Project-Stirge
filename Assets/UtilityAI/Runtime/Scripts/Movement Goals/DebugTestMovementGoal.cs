using Stirge.Combat;
using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI.MovementGoals
{
    public class DebugTestMovementGoal : MovementGoal, ISetupable<float, string, bool>
    {
        private float m_score;
        private string m_message;
        private bool m_useConsineInsteadOfSine;

        private float m_timer;

        public void Setup(float score, string message, bool useCosineInsteadOfSine)
        {
            m_score = score;
            m_message = message;
            m_useConsineInsteadOfSine = useCosineInsteadOfSine;
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
            if (m_useConsineInsteadOfSine)
                return m_score * Mathf.Cos(Time.time);
            else
                return m_score * Mathf.Sin(Time.time);
        }
    }
}
