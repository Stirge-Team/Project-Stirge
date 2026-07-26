using UnityEngine;

namespace Stirge.AI
{
    public class CircleTargetBehaviour : Behaviour
    {
        [SerializeField, Tooltip("If less than or equal to 0, will use the default speed on the NavMeshAgent on the prefab.")]
        private float m_speed;
        public override void _Enter(Agent agent)
        {
            if(m_speed > 0)
                agent.NavMeshAgent.speed = m_speed;

            base._Enter(agent);
        }
        public override void _Update(Agent agent, float deltaTime)
        {
            if(agent.TargetPosition != null && Vector3.Distance(agent.Transform.position, (Vector3)agent.TargetPosition) > agent.StoppingDistance)
            {
                float rndAngle = Random.Range(0,360) * Mathf.Deg2Rad;
                Vector3 angleAroundTarget = new Vector3(Mathf.Cos(rndAngle) * agent.StoppingDistance, 0, Mathf.Sin(rndAngle) * agent.StoppingDistance) + (Vector3)agent.TargetPosition;
                agent.NavMeshAgent.SetDestination(angleAroundTarget);
            }
        }
        public override void _Exit(Agent agent)
        {
            agent.ClearPath();

            if(m_speed > 0)
                agent.SetDefaultNavSpeed();
            
            base._Exit(agent);
        }
    }
}
