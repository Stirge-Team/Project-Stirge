using UnityEngine;

namespace Stirge.AI
{
    public class TimeCondition : Condition
    {
        [SerializeField] private float m_timer;
        private float m_currentTimer = 0;
        
        public override bool IsTrue(Agent agent)
        {
            if (m_currentTimer >= m_timer)
            {
                return true;
            }
            
            m_currentTimer += Time.deltaTime;
            return false;
        }
    }
}
