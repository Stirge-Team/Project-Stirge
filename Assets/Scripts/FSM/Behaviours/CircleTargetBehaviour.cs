using Stirge.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Stirge.AI
{
    [System.Serializable]
    public class CircleTargetBehaviour : MoveToTargetBehaviour
    {
        private float m_cachedStoppingDistance = 0;
        private float m_targetAngle = 0 ;
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);

            m_cachedStoppingDistance = agent.StoppingDistance;
            agent.NavMeshAgent.stoppingDistance = 0.1f; //removing the stopping distance

            m_targetAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
        }
        public override void _Update(Agent agent, float deltaTime)
        {
            //ok this needs a BIG update, but here is the idea
            // - have the ai check how many other ais there are
            // - figure out how to best spread themselves out (what angle)
            if (agent.TargetPosition != null)
            {
                Vector3 angleAroundTarget = new Vector3(Mathf.Cos(m_targetAngle) * m_cachedStoppingDistance, 0, Mathf.Sin(m_targetAngle) * m_cachedStoppingDistance) + (Vector3)agent.TargetPosition;
                agent.NavMeshAgent.SetDestination(angleAroundTarget);
            }
        }
        public override void _Exit(Agent agent)
        {
            agent.NavMeshAgent.stoppingDistance = m_cachedStoppingDistance;
            base._Exit(agent);
        }
    }
}
