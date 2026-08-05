using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.Combat
{
    using Attacks;

    public abstract class CombatEntity : MonoBehaviour
    {
        private static bool s_debug = true;

        [Header("Components")]
        [SerializeField] protected Rigidbody m_rb;
        [SerializeField] protected Animator m_anim;

        [Header("Combat Properties")]
        [SerializeField] protected EntityHealth m_health;
        public EntityHealth Health => m_health;

        protected Transform m_targetTransform;
        public Transform TargetTransform => m_targetTransform;

        protected bool m_isAttacking;
        public bool IsAttacking => m_isAttacking;

        [Header("Status")]
        [SerializeReference] protected List<TimedStatus> m_inflictedStatuses = new();
        
        protected bool m_isStunned;

        [Header("Ground Check Properties")]
        [SerializeField, Min(0)] protected float m_groundedCheckDistance;
        [SerializeField] protected LayerMask m_groundedCheckMask;

        #region UnityEvents
        private void Awake()
        {
            AwakeThis();
            m_director.stopped += OnAttackEnd;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateThis(deltaTime);

            UpdateStatuses(deltaTime);
        }

        protected virtual void AwakeThis() { }
        protected virtual void UpdateThis(float deltaTime) { }
        #endregion

        #region Transformation
        public virtual void ApplyRootMotion() { throw new System.NotImplementedException(); }

        protected virtual Vector3 GetPosition() { throw new System.NotImplementedException(); }
        protected virtual void SetPosition(Vector3 position) { throw new System.NotImplementedException(); }
        protected virtual Quaternion GetRotation() { throw new System.NotImplementedException(); }
        protected virtual void SetRotation(Quaternion rotation) { throw new System.NotImplementedException(); }
        protected virtual void SetRotation(Vector3 eulerRotation) { throw new System.NotImplementedException(); }
        public virtual Vector3 GetForward() { throw new System.NotImplementedException(); }
        #endregion

        #region Navigation
        public void SetTargetTransform(Transform target) => m_targetTransform = target;

        protected virtual void BeginGoToPosition(Vector3 newPosition) { throw new System.NotImplementedException(); }
        protected virtual void StopGoToPosition() { throw new System.NotImplementedException(); }

        protected virtual float GetMovementSpeed() { throw new System.NotImplementedException(); }
        protected virtual void SetMovementSpeed(float speed) { throw new System.NotImplementedException(); }
        protected virtual void ResetMovementSpeed() { throw new System.NotImplementedException(); }
        #endregion

        #region Physics
        public virtual bool IsGrounded() { throw new System.NotImplementedException(); }
        public virtual void ApplyPhysicsToTransform() { throw new System.NotImplementedException(); }
        #endregion

        #region Death State
        public void TakeDamage(int damage)
        {
            m_health.ModifyHealth(-Mathf.Abs(damage));
            OnDamageTaken(damage);
        }
        protected virtual void OnDamageTaken(int damage) { }

        public bool IsDead()
        {
            return m_health._isDead;
        }
        #endregion

        #region Statuses
        public void InflictStatus(Status status, CombatEntity attackingEntity)
        {
            // inflict the Status
            if (attackingEntity == null)
                status.OnInflict(this);
            else
                status.OnInflict(this, attackingEntity);   
        }
        public void InflictTimedStatus(TimedStatus status, CombatEntity attackingEntity)
        {
            // add to list to be updated
            switch (status.GetType().Name)
            {
                case nameof(Stun):
                    // only allow one Stun at a time
                    m_inflictedStatuses.RemoveAll(status => status.GetType() == typeof(Stun));

                    // add and inflict
                    Stun newStun = new(status as Stun);
                    if (attackingEntity == null)
                        newStun.OnInflict(this);
                    else
                        newStun.OnInflict(this, attackingEntity);

                    m_inflictedStatuses.Add(newStun);
                    break;
            }
        }

        private void UpdateStatuses(float deltaTime)
        {
            List<TimedStatus> toRemove = new();
            foreach (TimedStatus status in m_inflictedStatuses)
            {
                status.Update(this, deltaTime);
                if (status.IsCleared)
                {
                    status.OnClear(this);
                    toRemove.Add(status);
                    continue;
                }
            }

            if (toRemove.Count > 0)
                m_inflictedStatuses.RemoveAll(status => toRemove.Contains(status));
        }

        public bool GetIsStunned()
        {
            return m_isStunned;
        }
        public void SetIsStunned(bool value, float stunLength = 0)
        {
            m_isStunned = value;

            if (value)
                EnterStun(stunLength);
        }

        public abstract void EnterStun(float stunLength);
        public abstract void EnterKnockback(float strength, Vector3 direction, float height, float stunLength, bool m_ignoreGrounded);
        public abstract void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength, bool m_ignoreGrounded);
        #endregion

        #region Attacks
        [Header("Attacks")]
        // Director for playing Attack Timelines
        [SerializeField] private PlayableDirector m_director;

        public virtual void UseAttack(TimelineAsset attackTimeline)
        {
            StopAttacking();
            m_director.Play(attackTimeline);
            m_isAttacking = true;
        }

        public void OnAttackEnd(PlayableDirector director)
        {
            m_isAttacking = false;
        }

        public void StopAttacking()
        {
            if (m_isAttacking)
            {
                m_director.Stop();
            }

            // Set attacking to false
            m_isAttacking = false;
        }

        #region NodeLogic
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

            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator ApproachTarget(ApproachTargetNode node)
        {
            // init
            Vector3 targetPosition = m_targetTransform.position;

            // running
            BeginGoToPosition(targetPosition);
            SetMovementSpeed(node.Speed);
            bool withinRange = false;
            while (!withinRange)
            {
                // If not using initial position, update position
                if (!node.UseInitialPosition)
                {
                    targetPosition = m_targetTransform.position;
                    BeginGoToPosition(targetPosition);
                }

                // provide some leeway for vertical difference
                Vector3 currentPos = GetPosition();
                Vector3 endPos = targetPosition;
                if (Mathf.Abs(GetPosition().y - targetPosition.y) < 2f) // arbitrary value
                {
                    // if the y-axis distance between the current and target position is not too far
                    // treat the actual distance caculation as if they are on the same y-level
                    currentPos.y = 0;
                    endPos.y = 0;
                }
                if (Vector3.Distance(currentPos, endPos) <= node.StoppingDistance)
                {
                    withinRange = true;
                    break;
                }

                yield return null;
            }

            // exit
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator Translate(TranslateNode node)
        {
            // init
            Vector3 startPosition = GetPosition();
            Vector3 endPosition;

            if (node.IsLocalTranslation)
            {
                endPosition = startPosition + GetRotation() * node.Translation;
            }
            else
            {
                endPosition = startPosition + node.Translation;
            }

            float elapsedTime = 0f;
            bool arrived = false;

            // running
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
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator Delay(DelayNode node)
        {
            // init

            // running
            yield return new WaitForSeconds(node.Delay);

            // exit
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator TimedMove(TimedMoveNode node)
        {
            // init
            Vector3 startPosition = GetPosition();
            Vector3 endPosition = startPosition + GetRotation() * node.LocalOffset;

            float time = node.Time;
            float elapsedTime = 0f;
            float stoppingDistance = node.StoppingDistance;
            bool considerYPosition = node.ConsiderYPosition;

            bool arrived = false;

            // running
            yield return new WaitForFixedUpdate();
            while (!arrived)
            {
                float t = Mathf.Clamp01(elapsedTime / time);
                m_rb.MovePosition(Vector3.Lerp(startPosition, endPosition, t));

                ApplyPhysicsToTransform();

                Vector3 currentPos = m_rb.position;
                Vector3 targetPos = endPosition;
                if (!considerYPosition)
                {
                    currentPos.y = 0;
                    targetPos.y = 0;
                }

                // if arrived at end position
                if (elapsedTime >= time || Vector3.Distance(currentPos, targetPos) <= stoppingDistance)
                {
                    arrived = true;
                    break;
                }

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }        

            // exit
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator CurveMove(CurveMoveNode node)
        {
            // init
            Vector3 startPosition = GetPosition();
            Vector3 endPosition = startPosition + GetRotation() * node.LocalOffset;

            // WATCH: Unsure if last key in array will always be the key with the highest x/time value (the one at the end)
            float time = node.Curve.keys[^1].time;
            float elapsedTime = 0f;
            float stoppingDistance = node.StoppingDistance;
            bool considerYPosition = node.ConsiderYPosition;

            bool arrived = false;

            // running
            yield return new WaitForFixedUpdate();
            while (!arrived)
            {
                // divide by time to get normalised 0 - 1 t value as Lerp clamps t to 0 - 1
                float t = Mathf.Clamp(node.Curve.Evaluate(elapsedTime), 0, time) / time;
                m_rb.MovePosition(Vector3.Lerp(startPosition, endPosition, t));
                
                ApplyPhysicsToTransform();

                Vector3 currentPos = m_rb.position;
                Vector3 targetPos = endPosition;
                if (!considerYPosition)
                {
                    currentPos.y = 0;
                    targetPos.y = 0;
                }

                // if arrived at end position
                if (elapsedTime >= time || Vector3.Distance(currentPos, targetPos) <= stoppingDistance)
                {
                    arrived = true;
                    break;
                }

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            // exit
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator SpeedMove(SpeedMoveNode node)
        {
            // init
            Vector3 endPosition = GetPosition() + GetRotation() * node.LocalOffset;

            float speed = node.Speed;
            float stoppingDistance = node.StoppingDistance;
            bool considerYPosition = node.ConsiderYPosition;

            bool arrived = false;

            // running
            yield return new WaitForFixedUpdate();
            while (!arrived)
            {
                Vector3 target = Vector3.MoveTowards(GetPosition(), endPosition, speed * Time.fixedDeltaTime);
                m_rb.MovePosition(target);

                ApplyPhysicsToTransform();

                Vector3 currentPos = m_rb.position;
                Vector3 targetPos = endPosition;
                if (!considerYPosition)
                {
                    currentPos.y = 0;
                    targetPos.y = 0;
                }

                // if arrived at end position
                if (Vector3.Distance(currentPos, targetPos) <= stoppingDistance)
                {
                    arrived = true;
                    break;
                }
                yield return new WaitForFixedUpdate();
            }

            // exit
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator AccelerateMove(AccelerateMoveNode node)
        {
            // init
            Vector3 endPosition = GetPosition() + GetRotation() * node.LocalOffset;

            float acceleration = node.Acceleration;
            // If maxSpeed is not greater than 0, then there is no max speed
            float maxSpeed = node.MaxSpeed > 0 ? node.MaxSpeed : Mathf.Infinity;
            float stoppingDistance = node.StoppingDistance;
            bool considerYPosition = node.ConsiderYPosition;

            float currentSpeed = 0;
            bool arrived = false;

            // running
            yield return new WaitForFixedUpdate();
            while (!arrived)
            {
                // Increase speed by acceleration
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += acceleration * Time.fixedDeltaTime;
                    if (currentSpeed > maxSpeed)
                        currentSpeed = maxSpeed;
                }

                Vector3 target = Vector3.MoveTowards(GetPosition(), endPosition, currentSpeed * Time.fixedDeltaTime);
                m_rb.MovePosition(target);

                ApplyPhysicsToTransform();

                Vector3 currentPos = m_rb.position;
                Vector3 targetPos = endPosition;
                if (!considerYPosition)
                {
                    currentPos.y = 0;
                    targetPos.y = 0;
                }

                // if arrived at end position
                if (Vector3.Distance(currentPos, targetPos) <= stoppingDistance)
                {
                    arrived = true;
                    break;
                }
                yield return new WaitForFixedUpdate();
            }

            // exit
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        #endregion

        #endregion
    }
}
