using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat
{
    public abstract class CombatEntity : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected Animator m_anim;

        [Header("Combat Properties")]
        [SerializeField, Min(1)] protected int m_maxHealth;
        public bool isAttacking;

        protected int m_currentHealth;

        [Header("Status")]
        [SerializeField] protected List<Status> m_statuses = new();

        private float m_stunTime = 0;

        [Header("Ground Check Properties")]
        [SerializeField, Min(0)] protected float m_groundedCheckDistance;
        [SerializeField] protected LayerMask m_groundedCheckMask;

        #region UnityEvents
        private void Awake()
        {
            m_currentHealth = m_maxHealth;
            AwakeThis();
        }
        private void Update()
        {
            UpdateStatuses(Time.deltaTime);

            UpdateThis(Time.deltaTime);
        }

        protected virtual void AwakeThis() { }
        protected virtual void UpdateThis(float deltaTime) { }
        #endregion

        #region Death State
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

        #region Attacks
        public void UseAttack(string attackName)
        {
            if (m_anim.HasState(0, Animator.StringToHash(attackName)))
            {
                m_anim.Play(attackName);

                // get the length of the attack to play
                isAttacking = true;
            }
        }
        #endregion

        public abstract bool IsGrounded();

        #region Statuses
        public bool IsStunned()
        {
            return m_stunTime > 0;
        }

        public void InflictStatus(Status status)
        {
            m_statuses.Add(status);
        }

        private void UpdateStatuses(float deltaTime)
        {
            // update Stun
            if (m_stunTime > 0)
            {
                m_stunTime -= deltaTime;
                if (m_stunTime <= 0)
                {
                    m_stunTime = 0;
                    // no longer stunned
                }
            }
            
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
        public abstract bool EnterStun(float stunLength);
        public abstract bool EnterKnockback(float strength, Vector3 direction, float height, float stunLength);
        public abstract bool EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength);
        #endregion
    }
}
