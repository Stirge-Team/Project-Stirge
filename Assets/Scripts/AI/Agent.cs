using UnityEngine;

namespace Stirge.AI
{
    public class Agent : MonoBehaviour
    {
        [SerializeField] private FiniteStateMachine m_fsm;

        private void OnEnable()
        {
            m_fsm._Enter(this);
        }

        private void Update()
        {
            m_fsm._Update(this);
        }

        private void OnDisable()
        {
            m_fsm._Exit(this);
        }
    }
}
