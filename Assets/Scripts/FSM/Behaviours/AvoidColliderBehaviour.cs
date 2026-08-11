using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class AvoidColliderBehaviour : Behaviour
    {
        [SerializeField, Tooltip("How far this AI should be from any valid colliders")]
        private float m_clearance;
        [SerializeField, Tooltip("The layers that are used for the collision check")]
        private LayerMask m_targetLayers;
        private float m_cachedStoppingDistance;
        public override void _Enter(Agent agent)
        {
            m_cachedStoppingDistance = agent.StoppingDistance;
            base._Enter(agent);
        }
        public override void _Update(Agent agent, float deltaTime)
        {
            RaycastHit[] hits = Physics.SphereCastAll(agent.Transform.position, m_clearance, Vector3.zero, m_clearance, m_targetLayers);
            if (hits.Length > 0)
            {
                agent.NavMeshAgent.stoppingDistance = 0; //remove stopping distance
            }
            else
            {
                agent.NavMeshAgent.stoppingDistance = m_cachedStoppingDistance; //reset stopping distance
                return;
            }

            Vector3 desiredPosition = agent.Transform.position;
            foreach (var hitObject in hits)
            {
                Vector3 vector = hitObject.point - desiredPosition;
                float distance = vector.magnitude;
                if (distance > m_clearance)
                    continue;

                Vector3 direction = vector.normalized;
                desiredPosition += direction * (m_clearance - distance);
            }
            agent.NavMeshAgent.SetDestination(desiredPosition);
        }
    }
}
