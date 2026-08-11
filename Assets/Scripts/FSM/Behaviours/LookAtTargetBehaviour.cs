 using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class LookAtTargetBehaviour : Behaviour
    {
        [SerializeField, Min(0)] private float m_maxDegreesDelta;

        private bool m_prevHeadingBehaviour;

        public override void _Enter(Agent agent)
        {
            m_prevHeadingBehaviour = agent.Enemy.Motor.headingIsTargetPosition;
            agent.Enemy.Motor.ChangeHeadingBehaviour(true);
            base._Enter(agent);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            agent.Enemy.Motor.ChangeHeadingBehaviour(m_prevHeadingBehaviour);
            base._Exit(agent);
        }
    }
}
