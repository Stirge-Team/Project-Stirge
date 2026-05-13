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
            m_targetTransform = GameObject.FindWithTag("Player").transform;
        }
        protected override void UpdateThis(float deltaTime)
        {
            // check if enemy is dead this frame
            if (m_health._isDead)
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
            m_agent.FixedUpdate();
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

        #region Controls
        public override bool IsGrounded()
        {
            return Physics.Raycast(m_agent.Transform.position, Vector3.down, m_groundedCheckDistance, m_groundedCheckMask);
        }
        public override void ApplyRootMotion()
        {
            m_agent.ApplyRootMotion();
        }

        protected override Vector3 GetPosition()
        {
            return m_agent.Transform.position;
        }
        protected override void SetPosition(Vector3 newPosition)
        {
            m_agent.SetPosition(newPosition);
        }
        protected override Quaternion GetRotation()
        {
            return m_agent.Transform.rotation;
        }
        protected override void SetRotation(Quaternion newRotation)
        {
            m_agent.SetRotation(newRotation);
        }
        protected override void SetRotation(Vector3 eulerRotation)
        {
            m_agent.SetRotation(Quaternion.Euler(eulerRotation));
        }

        protected override void BeginGoToPosition(Vector3 newPosition)
        {
            m_agent.TargetPosition = newPosition;
            m_agent.SetPhysicsMode(PhysicsMode.NavMesh);
            m_agent.CalculatePath();
        }
        protected override void StopGoToPosition()
        {
            m_agent.TargetPosition = null;
            m_agent.ClearPath();
        }

        protected override float GetMovementSpeed()
        {
            return m_agent.GetNavSpeed();
        }
        protected override void SetMovementSpeed(float speed)
        {
            m_agent.SetNavSpeed(speed);
        }
        protected override void ResetMovementSpeed()
        {
            m_agent.SetDefaultNavSpeed();
        }
        #endregion

        #region Status
        public override void EnterStun(float length)
        {
            // different State for when Grounded
            if (IsGrounded())
                m_agent.EnterState(m_stunState);
            else
                m_agent.EnterState(m_airStunState);

            m_anim.Play("hitstun");
        }
        public override void EnterKnockback(float strength, Vector3 direction, float height, float stunLength)
        {
            InflictStatus(new Stun(stunLength));
            m_agent.EnterState(m_knockbackState);
            m_agent.ApplyKnockback(strength, direction, height);
            m_anim.Play("hitstun");
        }
        public override void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength)
        {
            InflictStatus(new Stun(stunLength));
            InflictStatus(new AirJuggle(strength, airStallLength));
            m_agent.EnterState(m_airJuggle);
            m_agent.ApplyKnockback(strength, direction);
            m_anim.Play("hitstun");
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
