using UnityEngine;

namespace Stirge.AI
{
    public class KnockbackBehaviour : OffGroundBehaviour
    {
        public override void _Enter(Agent agent)
        {
            agent.SetPhysicsMode(PhysicsMode.Physics);
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            base._Update(agent);
            
            if (offGround && agent.RetrieveMemory<bool>("Grounded"))
                agent.TriggerManualTransitions();
        }

        public override void _Exit(Agent agent)
        {
            agent.SetPhysicsMode(PhysicsMode.NavMesh);
            base._Exit(agent);
        }
    }
}
