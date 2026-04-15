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
            if (agent.TargetObject != null && Vector3.Distance(agent.transform.position, agent.TargetObject.position) <= agent.DetectionRadius)
            {
                agent.TargetPosition = agent.TargetObject.position;
            }
        }

        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
