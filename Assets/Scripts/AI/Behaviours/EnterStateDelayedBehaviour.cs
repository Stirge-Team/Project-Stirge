using UnityEngine;

namespace Stirge.AI
{
    public class EnterStateDelayedBehaviour : Behaviour
    {
        [SerializeField, Min(0)] private float m_delay;
        [SerializeField] private State m_targetState;

        private float m_timer;
        
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
            m_timer = m_delay;
        }

        public override void _Update(Agent agent)
        {
            if (m_timer >= 0)
            {
                m_timer -= Time.deltaTime;
                if (m_timer <= 0)
                {
                    agent.EnterState(m_targetState);
                }
            }
        }

        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
