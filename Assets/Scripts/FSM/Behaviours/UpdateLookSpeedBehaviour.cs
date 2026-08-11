using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class UpdateLookSpeedBehaviour : Behaviour
    {
        [SerializeField] private float m_newDegreesDelta;

        private float m_prevDegreesDelta;
        
        public override void _Enter(Agent agent)
        {
            m_prevDegreesDelta = agent.Enemy.Motor.angularSpeed;
            agent.Enemy.Motor.SetAngularSpeed(m_newDegreesDelta * Mathf.Deg2Rad);
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            agent.Enemy.Motor.SetAngularSpeed(m_prevDegreesDelta * Mathf.Deg2Rad);
        }
    }
}
