using System.Linq;
using UnityEngine;

namespace Stirge.AI
{
    public class FiniteStateMachine : Behaviour
    {
        [SerializeField] private State m_currentState;

        public override void _Enter(Agent agent)
        {
            m_currentState._Enter(agent);
        }
        public override void _Update(Agent agent)
        {
            m_currentState._Update(agent);

            foreach (Transition transition in m_currentState.Transitions)
            {
                // returns true if there are no Conditions and returns true for a Condition if it is empty/null
                if (transition.conditions.Length == 0 || transition.conditions.All(c => c == null || c.IsTrue(agent)))
                {
                    m_currentState._Exit(agent);
                    m_currentState = transition.targetState;
                    m_currentState._Enter(agent);
                    return;
                }
            }
        }
        public override void _Exit(Agent agent)
        {
            m_currentState._Exit(agent);
        }

        public void EnterState(Agent agent, State newState)
        {
            m_currentState._Exit(agent);
            m_currentState = newState;
            m_currentState._Enter(agent);
        }
    }
}
