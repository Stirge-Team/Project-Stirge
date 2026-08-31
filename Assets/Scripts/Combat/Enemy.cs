using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.Enemy
{
    using AI;
    using Combat;
    using Stirge.Combat.OldStatus;

    public class Enemy : CombatEntity
    {
        [Header("Enemy Properties")]
        [SerializeField] private Agent m_agent;
        [SerializeField] private EnemyMotor m_motor;

        [Header("Combat States")]
        [SerializeField] private State m_stunState;
        [SerializeField] private State m_airStunState;
        [SerializeField] private State m_knockbackState;
        [SerializeField] private State m_airJuggle;

        [HideInInspector] public EnemySpawner spawner = null;

        protected bool m_hasAttackToken = false;

        protected Transform m_targetTransform;
        public Transform TargetTransform => m_targetTransform;

        // properties
        public EnemyMotor Motor => m_motor;

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

            if (TargetTransform != null) //if there is a target
            {
                if (AttackTokenDispenser.instance != null)
                    AttackTokenDispenser.instance.EnterAttackRaffle(this, new ScoringMethods.DistanceScore(transform, TargetTransform)); //enter the raffle
                else
                    m_hasAttackToken = true;
            }

            m_agent.Update(deltaTime);
        }

        protected virtual void OnEnable()
        {
            m_agent.OnEnable();
        }
        protected virtual void OnDisable()
        {
            m_agent.OnDisable();
        }

        public override void UseAction(TimelineAsset attackTimeline)
        {
            if (m_hasAttackToken) //fail if no attack token
                base.UseAction(attackTimeline);
        }
        #endregion

        #region Attack Tokens
        public virtual bool GiveToken(float timeout = 0)
        {
            m_hasAttackToken = true;
            //remove token after given time, if any
            if (timeout > 0) StartCoroutine(TokenTimeout(timeout));
            return m_hasAttackToken;
        }
        private IEnumerator TokenTimeout(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            RemoveToken();
        }
        public virtual bool RemoveToken()
        {
            return m_hasAttackToken = false;
        }

        public virtual void LostRaffle()
        {
            Debug.Log($"[{name}]: dude i can't believe i lost the attack token raffle this is so sad :(", this);
        }
        #endregion

        #region Transformation
        public override Vector3 GetPosition()
        {
            return m_motor.transform.position;
        }
        public override void SetPosition(Vector3 newPosition)
        {
            m_motor.SetPosition(newPosition);
        }
        public override Quaternion GetRotation()
        {
            return m_motor.transform.rotation;
        }
        public override void SetRotation(Quaternion newRotation)
        {
            m_motor.SetRotation(newRotation);
        }
        public override void SetRotation(Vector3 eulerRotation)
        {
            m_motor.SetRotation(Quaternion.Euler(eulerRotation));
        }
        public override Vector3 GetForward()
        {
            return m_motor.transform.forward;
        }
        #endregion

        #region Physics
        public override bool IsGrounded()
        {
            return Physics.Raycast(m_agent.Transform.position, Vector3.down, m_groundedCheckDistance, m_groundedCheckMask);
        }
        public override void MovePosition(Vector3 newPosition)
        {
            m_motor.SetPosition(newPosition);
        }
        #endregion

        #region DeathState
        protected override void OnDamageTaken(int damage)
        {
        }
        #endregion

        #region Status
        public override void EnterStun(float stunLength)
        {
            m_isStunned = true;

            // different State for when Grounded
            if (IsGrounded())
                m_agent.EnterState(m_stunState);
            else
                m_agent.EnterState(m_airStunState);

            //m_anim.Play("hitstun");
        }
        public override void EnterKnockback(float strength, Vector3 direction, float height, float stunLength, bool ignoreGrounded)
        {
            if (IsGrounded() || ignoreGrounded)
            {
                if (stunLength > 0f)
                { 
                    //InflictStatus(new Stun(stunLength), null);
                }
                m_agent.EnterState(m_knockbackState);
                m_agent.ApplyKnockback(strength, direction, height);
                //m_anim.Play("hitstun");
            }
        }
        public override void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength, bool ignoreGrounded)
        {
            if (IsGrounded() || ignoreGrounded)
            {
                if (stunLength > 0f)
                {
                    //InflictTimedStatus(new Stun(stunLength), null);
                }
                m_agent.EnterState(m_airJuggle);
                m_agent.ApplyKnockback(strength, direction);
                //m_anim.Play("hitstun");
            }
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
