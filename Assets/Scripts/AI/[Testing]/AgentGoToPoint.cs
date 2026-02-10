using UnityEngine;
using UnityEngine.InputSystem;

namespace Stirge.AI
{
    public class AgentGoToPoint : MonoBehaviour
    {
        [SerializeField] private Agent[] m_agent;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 100))
                {
                    /*
                    foreach (Agent agent in m_agent)
                        agent.NavMeshAgent.SetDestination(hit.point);
                    */
                }
            }
        }
    }
}
