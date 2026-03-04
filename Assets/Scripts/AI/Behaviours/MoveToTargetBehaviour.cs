using UnityEngine;

namespace Stirge.AI
{
    public class MoveToTargetBehaviour : Behaviour
    {
        private Transform m_target;
        
        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
            m_target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        public override void _Update(Agent agent)
        {
            Vector3 currentPos = agent.transform.position;
            Vector3 targetPos = m_target.transform.position;

            Vector3 delta = targetPos - currentPos;

            // if target is within stopping distance, then do not calculate a new path
            if (delta.magnitude < agent.StoppingDistance)
                return;
            // otherwise set target to a position on a sphere with agent.StoppingDistance radius
            Vector3 newPosition = targetPos - delta.normalized * agent.StoppingDistance;

            agent.SetTargetPosition(newPosition);
        }

        public override void _Exit(Agent agent)
        {
            agent.ClearPath();
            base._Exit(agent);
        }
    }
}
