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

        #region Unity Events
        // PLEASE NOTE: Always call the BASE method first to avoid inconsistencies.
        // If Enemy updates first, it may use unupdated values of Health and states of Statuses such as Stun from the previous frame
        protected override void Awake()
        {
            base.Awake();
            m_agent.Awake();
        }
        protected override void Update()
        {
            base.Update();
            m_agent.Update(Time.deltaTime);
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
        private void ApplyStun(float length)
        {
            if (length > 0)
            {
                m_agent.WriteMemory("Stun", length);
            }
        }

        public override bool EnterStun(float length)
        {
            ApplyStun(length);
            if (m_agent.RetrieveMemory<bool>("Grounded"))
                m_agent.EnterState(m_stunState);
            else
                m_agent.EnterState(m_airStunState);

            return true;
        }
        public override bool EnterKnockback(float strength, Vector3 direction, float height, float stunLength)
        {
            ApplyStun(stunLength);
            m_agent.EnterState(m_knockbackState);
            m_agent.ApplyKnockback(strength, direction, height);

            return true;
        }
        public override bool EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength)
        {
            ApplyStun(stunLength);
            m_agent.WriteMemory("AirStall", airStallLength);
            m_agent.EnterState(m_airJuggle);
            m_agent.ApplyKnockback(strength, direction);

            return true;
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
