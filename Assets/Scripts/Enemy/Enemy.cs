using UnityEngine;

namespace Stirge.Enemy
{
    using AI;

    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Agent m_agent;
        
        [Header("Enemy Stats")]
        [SerializeField, Min(1)] private int m_maxHealth;
        private int m_currentHealth;

        [Header("Combat Details")]
        [SerializeField] private State m_stunState;
        [SerializeField] private State m_airStunState;
        [SerializeField] private State m_knockbackState;
        [SerializeField] private State m_airJuggle;

        [HideInInspector] public EnemySpawner spawner = null;

        private void Awake()
        {
            m_currentHealth = m_maxHealth;
            m_agent.Awake();
        }

        private void OnEnable()
        {
            m_agent.OnEnable();
        }

        private void Update()
        {
            // check if enemy is dead this frame
            if (IsDead())
            {
                if (spawner != null)
                    spawner.ReportDeath(this);
                Destroy(gameObject);
                return;
            }

            m_agent.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            m_agent.FixedUpdate(Time.deltaTime);
        }

        private void OnDisable()
        {
            m_agent.OnDisable();
        }

        public bool IsDead()
        {
            return m_currentHealth <= 0;
        }

        #region Combat
        public void TakeDamage(int damage)
        {
            m_currentHealth -= damage;
        }

        public void EnterStun(float length)
        {
            if (length > 0)
            {
                m_agent.WriteMemory("Stun", length);
            }
            if (m_agent.RetrieveMemory<bool>("Grounded"))
                m_agent.EnterState(m_stunState);
            else
                m_agent.EnterState(m_airStunState);
        }
        public void EnterKnockback(float strength, Vector2 direction, float height = 1f)
        {
            m_agent.EnterState(m_knockbackState);
            m_agent.ApplyKnockback(strength, direction, height);
        }
        public void EnterAirJuggle(float strength, Vector3 direction, float airStallLength)
        {
            m_agent.WriteMemory("AirStall", airStallLength);
            m_agent.EnterState(m_airJuggle);
            m_agent.ApplyKnockback(strength, direction);
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            m_agent.OnDrawGizmos();
        }
#endif
    }
}
