using Stirge.Tools;
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

            Vector3 desiredPosition = agent.Transform.position; //prep a target end position starting from the current position
            foreach (var hitObject in hits)
            {
                if(AbsoluteParent.SharedParent(new Transform[] {agent.Transform, hitObject.transform})) continue; //skip if we've hit ourself

                Vector3 vector = hitObject.point - desiredPosition; //get the direction and distance to the hit object
                float distance = vector.magnitude; //isolate the distance
                if (distance > m_clearance) //if outside the clearance range, skip the next steps
                    continue;

                Vector3 direction = vector.normalized; //isolate the direction
                desiredPosition += direction * (m_clearance - distance); //target position has now be moved away from the hit object so that the distance between them is outside the clearance range.
            }
            agent.NavMeshAgent.SetDestination(desiredPosition);
        }
    }
}
