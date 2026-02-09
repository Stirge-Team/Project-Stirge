using UnityEngine;

namespace Stirge.AI
{
    public class KnockbackBehaviour : Behaviour
    {
        public override void _Enter(Agent agent)
        {
            agent.SetPhysicsMode(true);
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            agent.SetPhysicsMode(false);
            base._Exit(agent);
        }
    }
}
