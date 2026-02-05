using UnityEngine;

namespace Stirge.AI
{
    [System.Serializable, CreateAssetMenu(fileName = "Transition", menuName = "StirgeAI/Transition", order = 1)]
    public class Transition : ScriptableObject
    {
        [Tooltip("If any of these are true, the Transition occurs.")]
        [SerializeField] private Condition[] m_conditions;

        [Tooltip("The State this Transition moves to.")]
        [SerializeField] private State m_targetState;

        public Condition[] conditions { get { return m_conditions; } }
        public State targetState { get { return m_targetState; } }
    }
}
