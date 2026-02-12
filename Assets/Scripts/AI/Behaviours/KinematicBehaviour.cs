using UnityEngine;

namespace Stirge.AI
{
    public class KinematicBehaviour : Behaviour
    {
        [SerializeField] private PhysicsMode m_exitMode;
        
        public override void _Enter(Agent agent)
        {
            agent.SetPhysicsMode(PhysicsMode.Kinematic);
        }

        public override void _Update(Agent agent)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            agent.SetPhysicsMode(m_exitMode);
        }
    }
}
