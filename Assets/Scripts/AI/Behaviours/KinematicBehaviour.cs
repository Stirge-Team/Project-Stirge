using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class KinematicBehaviour : Behaviour
    {
        [SerializeField] private PhysicsMode m_exitMode;
        
        public override void _Enter(Agent agent)
        {
            agent.SetPhysicsMode(PhysicsMode.Kinematic);
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
