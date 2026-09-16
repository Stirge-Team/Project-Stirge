using UnityEngine;
namespace Stirge.TestScript
{
    public class SimpleSpinner : MonoBehaviour
    {
        public float m_speed;
        void Update()
        {
            transform.Rotate(0, 0, m_speed);
        }
    }
}