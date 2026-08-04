using UnityEngine;

namespace Stirge.AI
{
    public class CircleTargetBehaviour : MoveToTargetBehaviour
    {
        public override void _Update(Agent agent, float deltaTime)
        {
            if(agent.TargetPosition != null && Vector3.Distance(agent.Transform.position, (Vector3)agent.TargetPosition) > agent.StoppingDistance)
            {
                float rndAngle = Random.Range(0,360) * Mathf.Deg2Rad;
                Vector3 angleAroundTarget = new Vector3(Mathf.Cos(rndAngle) * agent.StoppingDistance, 0, Mathf.Sin(rndAngle) * agent.StoppingDistance) + (Vector3)agent.TargetPosition;
                agent.NavMeshAgent.SetDestination(angleAroundTarget);
            }
        }
    }
}
