using System.Collections.Generic;
using UnityEngine;

namespace Stirge.AI
{
    [CreateAssetMenu(fileName = "State", menuName = "Stirge AI/State", order = 1)]
    public class State : ScriptableObject
    {       
        [SerializeField] private List<Behaviour> m_behaviours;
        [SerializeField] private List<Transition> m_transitions;

        [SerializeField] private State m_timedTransitionState;
        [SerializeField, Min(0)] private float m_timedTransitionDelay;
        private float m_transitionTimer;

        public List<Transition> Transitions { get { return m_transitions; } }

        public void _Enter(Agent agent)
        {
            m_transitionTimer = m_timedTransitionDelay;
            
            foreach (Behaviour behaviour in m_behaviours)
                behaviour._Enter(agent);

            Debug.Log($"Entered {name} State.");
        }
        public void _Update(Agent agent)
        {
            // if this state has a timed transition
            if (m_timedTransitionState != null)
            {
                m_transitionTimer -= Time.deltaTime;
                if (m_transitionTimer <= 0)
                {
                    agent.EnterState(m_timedTransitionState);
                    return;
                }
            }
            
            foreach (Behaviour behaviour in m_behaviours)
                behaviour._Update(agent);
        }
        public void _Exit(Agent agent)
        {
            foreach (Behaviour behaviour in m_behaviours)
                behaviour._Exit(agent);

            Debug.Log($"Exited {name} State.");
        }
    }
}
