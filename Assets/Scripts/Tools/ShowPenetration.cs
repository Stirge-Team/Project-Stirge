using UnityEngine;

namespace Stirge.Tools
{
    [ExecuteAlways]
    public class ShowPenetration : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float m_radius = 3f;
        [SerializeField, Range(1, 256)] private int m_maxNeighbours = 16;
        [Space]
        [SerializeField] private CapsuleCollider m_collider;
        private Collider[] m_neighbours;

        private void OnEnable()
        {
            m_neighbours = new Collider[m_maxNeighbours];
        }

        private void OnDrawGizmos()
        {
            if (m_collider == null)
                return;

            int count = Physics.OverlapSphereNonAlloc(transform.position, m_radius, m_neighbours);

            for (int i = 0; i < count; ++i)
            {
                Collider other = m_neighbours[i];
                if (!other || other == m_collider)
                    continue;

                if (Physics.ComputePenetration(
                        m_collider, transform.position, transform.rotation,
                        other, other.transform.position, other.transform.rotation,
                        out Vector3 direction, out float distance))
                {
                    // Draw penetration vector starting from the other collider's position.
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(other.transform.position, direction * distance);
                }
            }
        }
    }
}
