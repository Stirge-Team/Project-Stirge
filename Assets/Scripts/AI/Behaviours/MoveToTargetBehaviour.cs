using UnityEngine;

namespace Stirge.AI
{
    public class MoveToTargetBehaviour : Behaviour
    {
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            agent.CalculatePathToTarget();
        }

        public override void _Exit(Agent agent)
        {
            agent.ClearPath();
            base._Exit(agent);
        }
    }
}
