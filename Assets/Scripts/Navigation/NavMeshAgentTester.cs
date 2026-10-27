using UnityEngine;
using UnityEngine.AI;

namespace Stirge.Navigation
{
    public class NavMeshAgentTester : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent m_nav;
        [SerializeField] private Vector3[] m_targets;

        private int m_currentTarget = 0;

        private void Start()
        {
            m_nav.enabled = true;
            SetDestination();
        }

        private void Update()
        {
            m_nav.enabled = true;
            if (!m_nav.hasPath)
                SetDestination();

            if (ReachedTarget())
            {
                m_currentTarget++;
                if (m_currentTarget == m_targets.Length)
                {
                    m_currentTarget = 0;
                }
                SetDestination();
            }
        }

        private void SetDestination()
        {
            m_nav.SetDestination(m_targets[m_currentTarget]);
        }

        private bool ReachedTarget()
        {
            Vector3 target = m_targets[m_currentTarget];
            return Vector3.Distance(m_nav.transform.position, target) <= m_nav.stoppingDistance;
        }

        private void OnDrawGizmos()
        {
            foreach (Vector3 target in m_targets)
            {
                Gizmos.DrawCube(target, Vector3.one / 3f);
            }
        }
    }
}
