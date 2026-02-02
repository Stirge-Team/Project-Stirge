using UnityEngine;

namespace Stirge.AI
{
    public class FiniteStateMachine : MonoBehaviour
    {
        [SerializeField] private State m_currentState;

        // base methods
        public void _Enter(Agent agent)
        {
            m_currentState._Enter(agent);
        }
        public void _Update(Agent agent)
        {
            m_currentState._Update(agent);
                
            State newState = null;

            foreach (Transition transition in m_currentState.Transitions)
            {
                foreach (Condition condition in transition.conditions)
                {
                    if (condition.IsTrue(agent))
                    {
                        newState = transition.targetState;
                        break;
                    }
                }
            }

            if (newState != null)
            {
                m_currentState._Exit(agent);
                m_currentState = newState;
                m_currentState._Enter(agent);
            }
        }
        public void _Exit(Agent agent)
        {
            m_currentState._Exit(agent);
        }
    }
}
