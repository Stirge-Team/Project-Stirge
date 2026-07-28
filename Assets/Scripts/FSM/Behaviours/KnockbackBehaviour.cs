using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class KnockbackBehaviour : OffGroundBehaviour
    {
        public override void _Enter(Agent agent)
        {
            agent.SetPhysicsMode(PhysicsMode.Physics);
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            base._Update(agent, deltaTime);
        }

        public override void _Exit(Agent agent)
        {
            agent.SetPhysicsMode(PhysicsMode.NavMesh);
            base._Exit(agent);
        }
    }
}
