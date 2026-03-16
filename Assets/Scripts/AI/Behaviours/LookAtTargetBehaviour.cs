 using UnityEngine;

namespace Stirge.AI
{
    public class LookAtTargetBehaviour : Behaviour
    {
        [SerializeField, Min(0)] private float m_maxDegreesDelta;

        public override void _Enter(Agent agent)
        {
            base._Enter(agent);
        }

        public override void _Update(Agent agent)
        {
            if (agent.TargetPosition != null)
            {
                agent.RotateTowards((Vector3)agent.TargetPosition, m_maxDegreesDelta * Time.deltaTime);
            }
        }

        public override void _Exit(Agent agent)
        {
            base._Exit(agent);
        }
    }
}
