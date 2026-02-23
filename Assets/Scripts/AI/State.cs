using System.Collections.Generic;
using UnityEngine;

namespace Stirge.AI
{
    [CreateAssetMenu(fileName = "State", menuName = "StirgeAI/State", order = 1)]
    public class State : ScriptableObject
    {       
        [SerializeField] private List<Behaviour> m_behaviours;
        [SerializeField] private List<Transition> m_transitions;

        public List<Transition> Transitions { get { return m_transitions; } }

        public void _Enter(Agent agent)
        {
            foreach (Behaviour behaviour in m_behaviours)
                behaviour._Enter(agent);

            Debug.Log($"Entered {name} State.");
        }
        public void _Update(Agent agent)
        {
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
