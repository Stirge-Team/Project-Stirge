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
            m_prevDegreesDelta = agent.NavMeshAgent.angularSpeed;
            agent.NavMeshAgent.angularSpeed = m_newDegreesDelta;
        }

        public override void _Update(Agent agent, float deltaTime)
        {
            
        }

        public override void _Exit(Agent agent)
        {
            agent.NavMeshAgent.angularSpeed = m_prevDegreesDelta;
        }
    }
}
