using UnityEngine;

namespace Stirge.Enemy
{
    using AI;
    using Combat;

    public class Enemy : CombatEntity
    {
        [SerializeField] private Agent m_agent;
        
        [Header("Combat States")]
        [SerializeField] private State m_stunState;
        [SerializeField] private State m_airStunState;
        [SerializeField] private State m_knockbackState;
        [SerializeField] private State m_airJuggle;

        [HideInInspector] public EnemySpawner spawner = null;

        #region Unity Events
        // PLEASE NOTE: Always call the BASE method first to avoid inconsistencies.
        // If Enemy updates first, it may use unupdated values of Health and states of Statuses such as Stun from the previous frame
        protected override void AwakeThis()
        {
            m_agent.Awake();
        }
        protected override void UpdateThis(float deltaTime)
        {
            // check if enemy is dead this frame
            if (health._isDead)
            {
                if (spawner != null)
                    spawner.ReportDeath(this);
                Destroy(gameObject);
                return;
            }

            m_agent.Update(deltaTime);
        }

        private void FixedUpdate()
        {
            m_agent.FixedUpdate(Time.deltaTime);
        }
        private void OnEnable()
        {
            m_agent.OnEnable();
        }
        private void OnDisable()
        {
            m_agent.OnDisable();
        }
        #endregion

        #region CombatEntity
        public override bool m_isGrounded()
        {
            return Physics.Raycast(m_agent.Transform.position, Vector3.down, m_groundedCheckDistance, m_groundedCheckMask);
        }
        private void ApplyStun(float length)
        {
            if (length > 0)
            {
                m_agent.Enemy.InflictStatus(new Stun(length));
            }
        }

        public override bool EnterStun(float length)
        {
            ApplyStun(length);
            // different State for when Grounded
            if (m_isGrounded())
                m_agent.EnterState(m_stunState);
            else
                m_agent.EnterState(m_airStunState);

            m_anim.Play("hitstun");
            return true;
        }
        public override bool EnterKnockback(float strength, Vector3 direction, float height, float stunLength)
        {
            ApplyStun(stunLength);
            m_agent.EnterState(m_knockbackState);
            m_agent.ApplyKnockback(strength, direction, height);
            m_anim.Play("hitstun");

            return true;
        }
        public override bool EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength)
        {
            ApplyStun(stunLength);
            InflictStatus(new AirJuggle(strength, airStallLength));
            m_agent.EnterState(m_airJuggle);
            m_agent.ApplyKnockback(strength, direction);
            m_anim.Play("hitstun");

            return true;
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            m_agent.OnDrawGizmos();

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(m_agent.Transform.position, m_agent.Transform.position + Vector3.down * m_groundedCheckDistance);
        }
#endif
    }
}
