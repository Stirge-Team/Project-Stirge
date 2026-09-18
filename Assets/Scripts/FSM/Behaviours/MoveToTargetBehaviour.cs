using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class MoveToTargetBehaviour : Behaviour
    {       
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            if (agent.TargetPosition != null && Vector3.Distance(agent.Transform.position, (Vector3)agent.TargetPosition) > agent.StoppingDistance)
            {
                agent.CalculatePath();
            }
        }

        public override void _Exit(Agent agent)
        {
            agent.ClearPath();
            base._Exit(agent);
        }
    }
}
