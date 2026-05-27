using UnityEngine;

namespace Stirge.UtilityAI
{
    public class UtilityEnemy : MonoBehaviour
    {
        [SerializeField] private Transform m_target;

        private float m_speed;
        public float Speed => m_speed;

        public float maxHealth;

        private void Awake()
        {
            m_target = GameObject.FindWithTag("Player").transform;
        }
    }
}
