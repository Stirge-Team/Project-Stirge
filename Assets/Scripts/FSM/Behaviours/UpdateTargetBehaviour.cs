using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class UpdateTargetBehaviour : Behaviour
    {
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            // if the target is within range
            if (agent.TargetTransform != null && Vector3.Distance(agent.Transform.position, agent.TargetTransform.position) <= agent.TargetDetectionRadius)
            {
                agent.TargetPosition = agent.TargetTransform.position;
            }
        }

        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
