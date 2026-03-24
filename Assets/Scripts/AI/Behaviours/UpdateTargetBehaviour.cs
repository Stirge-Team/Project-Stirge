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
            Transform target = agent.RetrieveMemory<Transform>("TargetTransform");
            if (target != null && Vector3.Distance(agent.transform.position, target.position) <= agent.DetectionRadius)
            {
                agent.TargetPosition = target.position;
            }
        }

        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
