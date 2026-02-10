using Stirge.AI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stirge.Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Agent m_agent;
        
        [Header("Enemy Stats")]
        [SerializeField, Min(1)] private int m_maxHealth;
        private int m_currentHealth;

        [Header("Combat Details")]
        public EnemySpawner spawner = null;

        private void Start()
        {
            m_currentHealth = m_maxHealth;
        }

        private void Update()
        {
            if (IsDead())
            {
                if (spawner != null)
                    spawner.ReportDeath();
                Destroy(gameObject);
            }
        }

        private bool IsDead()
        {
            return m_currentHealth <= 0;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
                m_currentHealth--;
        }
    }
}
