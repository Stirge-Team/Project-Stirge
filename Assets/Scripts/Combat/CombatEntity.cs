using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat
{
    public abstract class CombatEntity : MonoBehaviour
    {
        [Header("Combat Details")]
        [SerializeField, Min(1)] protected int m_maxHealth;
        protected int m_currentHealth;

        [HideInInspector] public CombatEntitySpawner spawner = null;

        [SerializeField] protected List<Status> m_statuses = new();

        #region UnityEvents
        protected virtual void Awake()
        {
            m_currentHealth = m_maxHealth;
        }
        protected virtual void Update()
        {
            // check if enemy is dead this frame
            if (IsDead())
            {
                if (spawner != null)
                    spawner.ReportDeath(this);
                Destroy(gameObject);
                return;
            }

            // update Statuses
            foreach (Status status in m_statuses)
            {
                if (status.IsCleared)
                {
                    m_statuses.Remove(status);
                    continue;
                }
                status.Update(this);
            }
        }
        #endregion

        #region Damage
        public void TakeDamage(int damage)
        {
            m_currentHealth -= damage;
            OnDamageTaken(damage);
        }
        protected virtual void OnDamageTaken(int damage) { }

        public bool IsDead()
        {
            return m_currentHealth <= 0;
        }
        #endregion

        #region Statuses
        public abstract bool EnterStun(float stunLength);
        public abstract bool EnterKnockback(float strength, Vector3 direction, float height, float stunLength);
        public abstract bool EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength);
        #endregion
    }
}
