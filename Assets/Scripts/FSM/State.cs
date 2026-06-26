using UnityEngine;

namespace Stirge.AI
{
    [CreateAssetMenu(fileName = "New State", menuName = "Stirge/AI State", order = 1)]
    public class State : ScriptableObject
    {       
        [SerializeReference] private Behaviour[] m_behaviours = new Behaviour[0];
        [SerializeField] private Transition[] m_transitions;

        [SerializeField] private State m_timedTransitionState;
        [SerializeField, Min(0)] private float m_timedTransitionDelay;
        private float m_transitionTimer;

        public Transition[] Transitions { get { return m_transitions; } }

        public void _Enter(Agent agent)
        {
            foreach (Behaviour behaviour in m_behaviours)
                behaviour._Enter(agent);
            
            m_transitionTimer = m_timedTransitionDelay;

            Debug.Log($"Entered {name} State.");
        }
        public void _Update(Agent agent, float deltaTime)
        {
            foreach (Behaviour behaviour in m_behaviours)
                behaviour._Update(agent, deltaTime);
            
            // if this state has a timed transition
            if (m_timedTransitionState != null)
            {
                m_transitionTimer -= deltaTime;
                if (m_transitionTimer <= 0)
                {
                    agent.EnterState(m_timedTransitionState);
                    return;
                }
            }
        }
        public void _Exit(Agent agent)
        {
            foreach (Behaviour behaviour in m_behaviours)
                behaviour._Exit(agent);

            Debug.Log($"Exited {name} State.");
        }
    }
}
