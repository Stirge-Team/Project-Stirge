using UnityEngine;

namespace Stirge.TestScript
{
    public class KinematicMoveTest : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rb;

        private void Start()
        {
            m_rb.isKinematic = !m_rb.isKinematic;
        }

        private void Update()
        {
            m_rb.MovePosition(transform.position + (transform.forward * Time.deltaTime));
        }
    }
}
