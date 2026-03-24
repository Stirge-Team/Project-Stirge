using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class StunBehaviour : Behaviour
    {
        private float m_stunTimer;
        
        public override void _Enter(Agent agent)
        {
            if (agent.ContainsMemory("Stun"))
                m_stunTimer = agent.RetrieveMemory<float>("Stun");
        }
        public override void _Update(Agent agent, float deltaTime)
        {
            // update stun
            if (m_stunTimer > 0)
            {
                m_stunTimer -= deltaTime;
                if (m_stunTimer <= 0)
                {
                    m_stunTimer = 0;
                    agent.RemoveMemory("Stun");
                }
            }
        }
        public override void _Exit(Agent agent)
        {
            agent.RemoveMemory("Stun");
        }
    }
}
