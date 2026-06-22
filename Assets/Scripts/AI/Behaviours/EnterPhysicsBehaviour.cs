using UnityEngine;

namespace Stirge.AI
{
    public class EnterPhysicsBehaviour : Behaviour
    {
        [SerializeField] private PhysicsMode m_newMode;
        [SerializeField] private bool m_returnToOldModeOnExit;

        private PhysicsMode m_oldMode;

        public override void _Enter(Agent agent)
        {
            if (m_returnToOldModeOnExit)
                m_oldMode = agent.PhysicsMode;
            agent.SetPhysicsMode(m_newMode);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            if (m_returnToOldModeOnExit)
            {
                agent.SetPhysicsMode(m_oldMode);
            }
        }
    }
}
