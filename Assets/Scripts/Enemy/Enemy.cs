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
        [SerializeField] private State m_stunState;
        [SerializeField] private State m_knockbackState;
        [SerializeField] private State m_airJuggle;

        [HideInInspector] public EnemySpawner spawner = null;

        private float m_stunTimer;

        private void Start()
        {
            m_currentHealth = m_maxHealth;
            m_agent.Start();
        }

        private void OnEnable()
        {
            m_agent.OnEnable();
        }

        private void Update()
        {
            if (IsDead())
            {
                if (spawner != null)
                    spawner.ReportDeath();
                Destroy(gameObject);
                return;
            }

            // update stun
            if (m_stunTimer > 0)
            {
                m_stunTimer -= Time.deltaTime;
                if (m_stunTimer <= 0)
                {
                    m_stunTimer = 0;
                    m_agent.WriteMemory("Stun", false);
                }
            }

            m_agent.Update();
        }

        private void FixedUpdate()
        {
            m_agent.FixedUpdate();
        }

        private void OnDisable()
        {
            m_agent.OnDisable();
        }

        private bool IsDead()
        {
            return m_currentHealth <= 0;
        }

        #region Combat
        private void ApplyStun(float length)
        {
            if (length > 0)
            {
                m_stunTimer = length;
                m_agent.WriteMemory("Stun", true);
            }
        }
        public void EnterStun(float length)
        {
            ApplyStun(length);
            m_agent.EnterState(m_stunState);
        }
        public void EnterKnockback(float strength, Vector2 direction, float stunLength, float height = 1f)
        {
            ApplyStun(stunLength);
            m_agent.EnterState(m_knockbackState);
            m_agent.ApplyKnockback(strength, direction, height);
        }
        public void EnterAirJuggle(float strength, Vector3 direction, float stunLength, float airStallLength)
        {
            ApplyStun(stunLength);
            m_agent.WriteMemory("AirStallLength", airStallLength);
            m_agent.EnterState(m_airJuggle);
            m_agent.ApplyKnockback(strength, direction);
        }
        #endregion

#if UNITY_EDITOR
        public void DebugStun(InputAction.CallbackContext context)
        {
            if (context.started)
                EnterStun(3f);
        }

        public void DebugKnockback(InputAction.CallbackContext context)
        {
            if (context.started)
                EnterKnockback(500f, new Vector2(1, 1), 3f);
        }
        public void DebugReduceHealth(InputAction.CallbackContext context)
        {
            if (context.started)
                m_currentHealth--;
        }
        public void DebugAirJuggle(InputAction.CallbackContext context)
        {
            if (context.started)
                EnterAirJuggle(300f, Vector3.up, 0f, 1f);
        }

        private void OnDrawGizmos()
        {
            m_agent.OnDrawGizmos();
        }
#endif
    }
}
