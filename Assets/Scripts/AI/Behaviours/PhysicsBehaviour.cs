using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class PhysicsBehaviour : Behaviour
    {
        [SerializeField] private bool m_maintainPriorMode = false;
        [SerializeField] private PhysicsMode m_enterMode;
        [SerializeField] private PhysicsMode m_exitMode;
        
        public override void _Enter(Agent agent)
        {
            if (!m_maintainPriorMode)
                agent.SetPhysicsMode(m_enterMode);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            agent.SetPhysicsMode(m_exitMode);
        }
    }
}
