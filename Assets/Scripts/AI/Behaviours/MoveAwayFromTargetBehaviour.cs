using UnityEngine;

namespace Stirge.AI
{

    [System.Serializable]
    public class MoveAwayFromTargetBehaviour : MoveToTargetBehaviour
    {
        public override void _Update(Agent agent, float deltaTime)
        {
            if(agent.TargetPosition != null && Vector3.Distance(agent.Transform.position, (Vector3)agent.TargetPosition) <= agent.StoppingDistance)
            {
                Vector3 directionToTarget = (agent.Transform.position - (Vector3)agent.TargetPosition).normalized;
                Vector3 destination = (Vector3)agent.TargetPosition + directionToTarget * -agent.StoppingDistance;
                agent.NavMeshAgent.SetDestination(destination);
            }
        }
    }
}