using System.Collections.Generic;
using Stirge.Management;
using UnityEngine;
using System.Collections;

namespace Stirge.Combat
{
    using Attacks;

    public abstract class CombatEntity : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected Animator m_anim;

        [Header("Combat Properties")]
        [SerializeField] protected EntityHealth m_health;
        public EntityHealth Health => m_health;

        //[SerializeField, Min(1)] protected int m_maxHealth;
        //protected int m_currentHealth;
        protected bool m_isAttacking;

        public bool IsAttacking => m_isAttacking;

        [Header("Status")]
        [SerializeField] protected List<Status> m_statuses = new();

        private float m_stunTime = 0;

        [Header("Ground Check Properties")]
        [SerializeField, Min(0)] protected float m_groundedCheckDistance;
        [SerializeField] protected LayerMask m_groundedCheckMask;

        #region UnityEvents
        private void Awake()
        {
            AwakeThis();
        }
        private void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateThis(deltaTime);

            UpdateStatuses(deltaTime);

            if (m_isAttacking)
                UpdateAttacking(deltaTime);
        }

        protected virtual void AwakeThis() { }
        protected virtual void UpdateThis(float deltaTime) { }
        #endregion

        #region Controls
        protected Transform m_targetTransform;

        public void SetTargetTransform(Transform target) => m_targetTransform = target;
        public Transform TargetTransform => m_targetTransform;
        public abstract bool IsGrounded();
        public abstract void ApplyRootMotion();

        protected abstract Vector3 GetPosition();
        protected abstract void SetPosition(Vector3 position);
        protected abstract Quaternion GetRotation();
        protected abstract void SetRotation(Quaternion rotation);
        protected abstract void SetRotation(Vector3 eulerRotation);

        protected abstract void GoToPosition(Vector3 newPosition, float speed = 0);
        protected abstract void StopGoToPosition();

        protected abstract float GetMovementSpeed();
        protected abstract void SetMovementSpeed(float speed);
        protected abstract void ResetMovementSpeed();
        #endregion

        #region Death State
        public void TakeDamage(int damage)
        {
            m_health.ModifyHealth(damage);
            OnDamageTaken(damage);
        }
        protected virtual void OnDamageTaken(int damage) { }

        public bool IsDead()
        {
            return m_health._isDead;
        }
        #endregion

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

        #region Attacks
        private AttackNode[] m_attackSequence;
        private AttackNode m_currentAttackNode;
        private int m_currentAttackIndex;
        private Coroutine m_currentAttackCoroutine;

        public void UseAttack(AttackData attackData)
        {
            m_attackSequence = attackData.EvaluateSequence();
            m_currentAttackNode = null;
            m_currentAttackIndex = -1;
            m_isAttacking = true;
        }

        private void UpdateAttacking(float deltaTime)
        {
            // if no node is currently being processed
            if (m_currentAttackNode == null)
            {
                m_currentAttackIndex++;

                // if reached the end of the sequence, exit this state
                if (m_currentAttackIndex >= m_attackSequence.Length)
                {
                    m_currentAttackCoroutine = null;
                    m_isAttacking = false;
                    return;
                }

                // start processing the new AttackNode
                m_currentAttackNode = m_attackSequence[m_currentAttackIndex];
                StartAttackCoroutine();
            }
        }

        public void StopAttacking()
        {
            if (m_isAttacking)
            {
                StopAttackCoroutine();
            }
        }

        private void StartAttackCoroutine()
        {
            Debug.Log($"Beginning processing {m_currentAttackNode.GetType().Name}.");
            switch (m_currentAttackNode.GetType().Name)
            {
                case nameof(AnimationNode):
                    m_currentAttackCoroutine = StartCoroutine(PlayAnimation(m_currentAttackNode as AnimationNode));
                    break;
                case nameof(ApproachTargetNode):
                    m_currentAttackCoroutine = StartCoroutine(ApproachTarget(m_currentAttackNode as ApproachTargetNode));
                    break;
                case nameof(TranslateNode):
                    m_currentAttackCoroutine = StartCoroutine(Translate(m_currentAttackNode as TranslateNode));
                    break;
                case nameof(DelayNode):
                    m_currentAttackCoroutine = StartCoroutine(Delay(m_currentAttackNode as DelayNode));
                    break;
            }
        }
        private void StopAttackCoroutine()
        {
            // Still need to check for null values as no idea when Coroutine will necessarily update,
            // so isAttacking may still be true when one of these values is null
            if (m_currentAttackCoroutine != null)
                StopCoroutine(m_currentAttackCoroutine);

            if (m_currentAttackNode != null)
            {
                switch (m_currentAttackNode.GetType().ToString())
                {
                    case nameof(AnimationNode):
                        // reset animator component
                        m_anim.speed = 1;
                        m_anim.StopPlayback();

                        // apply motion from animation
                        AnimationNode animationNode = m_currentAttackNode as AnimationNode;
                        if (animationNode.HasRootMotion)
                            ApplyRootMotion();

                        break;
                    case nameof(ApproachTargetNode):
                        ResetMovementSpeed();
                        StopGoToPosition();
                        break;
                }
            }
        }

        private IEnumerator PlayAnimation(AnimationNode node)
        {
            // If there are issues with animator speed, check this first
            // init
            m_anim.speed = node.Speed;
            m_anim.Play(node.AnimationStateName);

            // running
            yield return new WaitForSeconds(node.Time);

            // exit
            m_anim.speed = 1;
            if (node.HasRootMotion)
                ApplyRootMotion();

            m_currentAttackNode = null;
            Debug.Log($"Finished processing Animation Node.");
        }
        private IEnumerator ApproachTarget(ApproachTargetNode node)
        {
            // init
            Vector3 targetPosition = m_targetTransform.position;

            // running
            GoToPosition(targetPosition, node.Speed);
            bool withinRange = false;
            while (!withinRange)
            {
                // If not using initial position, update position
                if (!node.UseInitialPosition)
                {
                    targetPosition = m_targetTransform.position;
                    GoToPosition(targetPosition, node.Speed);
                }

                if (Vector3.Distance(GetPosition(), targetPosition) <= node.StoppingDistance)
                {
                    withinRange = true;
                    break;
                }

                yield return null;
            }

            // exit
            
            m_currentAttackNode = null;
            Debug.Log($"Finished processing Approach Target node.");
        }
        private IEnumerator Translate(TranslateNode node)
        {
            // init
            Vector3 startPosition = GetPosition();
            Vector3 endPosition;

            if (node.IsLocalTranslation)
            {
                endPosition = GetPosition() + GetRotation() * node.Translation;
            }
            else
            {
                endPosition = GetPosition() + node.Translation;
            }

            // running
            float elapsedTime = 0f;
            bool arrived = false;
            while (!arrived)
            {
                float t = Mathf.Clamp01(elapsedTime / node.Time);
                SetPosition(Vector3.Lerp(startPosition, endPosition, t));

                // if arrived at end position
                if (t >= 1)
                {
                    arrived = true;
                    break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // exit
            m_currentAttackNode = null;
            Debug.Log($"Finished processing Translate Node.");
        }
        private IEnumerator Delay(DelayNode node)
        {
            // init

            // running
            yield return new WaitForSeconds(node.Delay);

            // exit
            m_currentAttackNode = null;
            Debug.Log($"Finished processing Delay Node.");
        }
        #endregion
    }
}
