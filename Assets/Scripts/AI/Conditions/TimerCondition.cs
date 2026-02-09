using UnityEngine;

namespace Stirge.AI
{
    public class TimerCondition : Condition
    {
        [SerializeField] private float m_timer;
        private float m_currentTimer = 0;
        
        public override bool IsTrue(Agent agent)
        {
            if (m_currentTimer >= m_timer)
            {
                m_currentTimer = 0;
                return true;
            }
            
            m_currentTimer += Time.deltaTime;
            return false;
        }
    }
}
