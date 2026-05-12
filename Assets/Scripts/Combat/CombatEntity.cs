using System.Collections.Generic;
using Stirge.Management;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Stirge.Combat
{
    using Attacks;
    using Tools;

    public abstract class CombatEntity : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected Animator m_anim;

        [Header("Combat Properties")]
        public bool isAttacking;
        [SerializeField]
        protected EntityHealth m_health;
        public EntityHealth health => m_health;

        //[SerializeField, Min(1)] protected int m_maxHealth;
        //protected int m_currentHealth;

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
            UpdateStatuses(Time.deltaTime);

            UpdateThis(Time.deltaTime);
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
        #endregion

        #region Death State
        public void TakeDamage(int damage)
        {
            m_health.ModifyHealth(damage);
            OnDamageTaken(damage);
        }
        protected virtual void OnDamageTaken(int damage) { }
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

        public abstract bool m_isGrounded();

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
        private Coroutine m_currentAttackCoroutine;

        public void StartAttackCoroutine(Ref<AttackNode> nodeRef)
        {
            Debug.Log($"Beginning processing {nodeRef.Value.GetType().Name}.");
            switch (nodeRef.Value.GetType().Name)
            {
                case nameof(AnimationNode):
                    m_currentAttackCoroutine = StartCoroutine(PlayAnimation(nodeRef));
                    break;
                case nameof(ApproachTargetNode):
                    m_currentAttackCoroutine = StartCoroutine(ApproachTarget(nodeRef));
                    break;
                case nameof(TranslateNode):
                    m_currentAttackCoroutine = StartCoroutine(Translate(nodeRef));
                    break;
                case nameof(DelayNode):
                    m_currentAttackCoroutine = StartCoroutine(Delay(nodeRef));
                    break;
            }
        }
        public void StopAttackCoroutine(AttackNode node)
        {
            if (m_currentAttackCoroutine != null)
            StopCoroutine(m_currentAttackCoroutine);

            switch (node.GetType().ToString())
            {
                case nameof(AnimationNode):
                    // reset animator component
                    m_anim.speed = 1;
                    m_anim.StopPlayback();

                    // apply motion from animation
                    AnimationNode animationNode = (AnimationNode)node;
                    if (animationNode.HasRootMotion)
                        ApplyRootMotion();

                    break;
            }
        }

        private IEnumerator PlayAnimation(Ref<AttackNode> nodeRef)
        {
            // If there are issues with animator speed, check this first
            // init
            AnimationNode castNode = (AnimationNode)nodeRef.Value;
            m_anim.speed = castNode.Speed;
            m_anim.Play(castNode.Animation.name);

            // running
            yield return new WaitForSeconds(castNode.EvaluateTime());

            // exit
            m_anim.speed = 1;
            if (castNode.HasRootMotion)
                ApplyRootMotion();

            nodeRef.SetNull();
            Debug.Log($"Finished processing Animation Node.");
        }
        private IEnumerator ApproachTarget(Ref<AttackNode> nodeRef)
        {
            // init
            ApproachTargetNode castNode = (ApproachTargetNode)nodeRef.Value;
            Vector3 targetPosition = m_targetTransform.position;

            // running
            bool withinRange = false;
            while (!withinRange)
            {
                // If not using initial position, update position
                if (!castNode.UseInitialPosition)
                    targetPosition = m_targetTransform.position;

                SetPosition(Vector3.MoveTowards(GetPosition(), targetPosition, castNode.Speed * Time.deltaTime));

                if (Vector3.Distance(GetPosition(), targetPosition) <= castNode.StoppingDistance)
                {
                    withinRange = true;
                    break;
                }

                yield return null;
            }

            // exit
            nodeRef.SetNull();
            Debug.Log($"Finished processing Approach Target castNode.");
        }
        private IEnumerator Translate(Ref<AttackNode> nodeRef)
        {
            // init
            TranslateNode castNode = (TranslateNode)nodeRef.Value;
            Vector3 startPosition = GetPosition();
            Vector3 endPosition;

            if (castNode.IsLocalTranslation)
            {
                endPosition = GetPosition() + GetRotation() * castNode.Translation;
            }
            else
            {
                endPosition = GetPosition() + castNode.Translation;
            }

            // running
            float elapsedTime = 0f;
            bool arrived = false;
            while (!arrived)
            {
                float t = Mathf.Clamp01(elapsedTime / castNode.Time);
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
            nodeRef.SetNull();
            Debug.Log($"Finished processing Translate Node.");
        }
        private IEnumerator Delay(Ref<AttackNode> nodeRef)
        {
            // init
            DelayNode castNode = (DelayNode)nodeRef.Value;
            
            // running
            yield return new WaitForSeconds(castNode.Delay);

            // exit
            nodeRef.SetNull();
            Debug.Log($"Finished processing Delay Node.");
        }
        #endregion
    }
}
