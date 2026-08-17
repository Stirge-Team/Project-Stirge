 using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class LookAtTargetBehaviour : Behaviour
    {
        [SerializeField, Min(0)] private float m_maxDegreesDelta;

        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            if (agent.TargetPosition != null)
            {
                agent.RotateTowards((Vector3)agent.TargetPosition, m_maxDegreesDelta * deltaTime);
            }
            else if (agent.TargetTransform != null)
            {
                agent.RotateTowards(agent.TargetTransform.position, m_maxDegreesDelta * deltaTime);
            }
        }

        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
