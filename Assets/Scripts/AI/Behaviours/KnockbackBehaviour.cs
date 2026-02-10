using UnityEngine;

namespace Stirge.AI
{
    public class KnockbackBehaviour : OffGroundBehaviour
    {
        public override void _Enter(Agent agent)
        {
            agent.SetPhysicsMode(true);
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            base._Update(agent);
            
            if (offGround) agent.TriggerManualTransitions();
        }

        public override void _Exit(Agent agent)
        {
            agent.SetPhysicsMode(false);
            base._Exit(agent);
        }
    }
}
