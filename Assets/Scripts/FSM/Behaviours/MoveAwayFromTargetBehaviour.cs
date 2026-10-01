using UnityEngine;

namespace Stirge.AI
{

    [System.Serializable]
    public class MoveAwayFromTargetBehaviour : MoveToTargetBehaviour
    {
        private float m_cachedStoppingDistance = 0;
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);

            m_cachedStoppingDistance = agent.StoppingDistance;
            agent.NavMeshAgent.stoppingDistance = 0;
        }
        public override void _Update(Agent agent, float deltaTime)
        {
            if(agent.TargetPosition != null)
            {
                Vector3 directionToTarget = (agent.Transform.position - (Vector3)agent.TargetPosition).normalized;
                Vector3 destination = (Vector3)agent.TargetPosition + directionToTarget * m_cachedStoppingDistance;
                agent.NavMeshAgent.SetDestination(destination);
            }
        }

        public override void _Exit(Agent agent)
        {
            agent.NavMeshAgent.stoppingDistance = m_cachedStoppingDistance;
            base._Exit(agent);
        }
    }
}