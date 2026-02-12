using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable]
    public class Transition
    {
        [Tooltip("The State this Transition moves to.")]
        [SerializeField] private State m_targetState;

        [Tooltip("If any of these are true, the Transition occurs.")]
        [SerializeField] private Condition[] m_conditions = new Condition[0];

        public State targetState { get { return m_targetState; } }
        public Condition[] conditions { get { return m_conditions; } }
    }
}
