using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.Combat
{
    using UtilityAI;
    using System;

    public enum ModifierType
    {
        Additive,
        Multiplicative
    }
    
    public abstract class CombatEntity : MonoBehaviour
    {
        private static bool s_debug = true;

        [Header("Combat Components")]
        [SerializeField] protected EntityHealth m_health;
        // Director for playing Attack Timelines
        [SerializeField] private PlayableDirector m_director;

        public EntityHealth Health => m_health;

        protected bool m_isPerformingAction;
        public bool IsPerformingAction => m_isPerformingAction;

        [Header("Ground Check Properties")]
        [SerializeField, Min(0)] protected float m_groundedCheckDistance;
        [SerializeField] protected LayerMask m_groundedCheckMask;

        // status
        protected List<Status> m_inflictedStatuses = new();
        protected bool m_isStunned;

        #region UnityEvents
        private void Awake()
        {
            AwakeThis();
            m_director.stopped += OnActionEnd;
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
        public virtual Vector3 GetPosition() { throw new System.NotImplementedException(); }
        public virtual void SetPosition(Vector3 position) { throw new System.NotImplementedException(); }
        public virtual Quaternion GetRotation() { throw new System.NotImplementedException(); }
        public virtual void SetRotation(Quaternion rotation) { throw new System.NotImplementedException(); }
        public virtual void SetRotation(Vector3 eulerRotation) { throw new System.NotImplementedException(); }
        public virtual Vector3 GetForward() { throw new System.NotImplementedException(); }
        #endregion

        #region Physics
        public virtual bool IsGrounded() { throw new System.NotImplementedException(); }
        /// <summary>
        /// Move to position with respect to Physics.
        /// </summary>
        /// <param name="newPosition"></param>
        public virtual void MovePosition(Vector3 newPosition) { throw new System.NotImplementedException(); }
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
        public virtual void InflictStatus(Status status, CombatEntity user)
        {
            // inflict the Status
            status.OnApply(user, this);
        }

        private void UpdateStatuses(float deltaTime)
        {
            List<int> toRemove = new();
            int index = 0;
            foreach (Status status in m_inflictedStatuses)
            {
                if (status.Update(this))
                {
                    status.OnClear(this);
                    toRemove.Add(index);
                }
                index++;
            }

            // remove backwards to avoid indicies from changing before removal
            for (int count = toRemove.Count, i = count - 1; i >= 0; i--)
            {
                m_inflictedStatuses.RemoveAt(toRemove[i]);
            }
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

        public virtual void EnterStun(float stunLength) { throw new NotImplementedException(); }
        public virtual void EnterKnockback(float strength, Vector3 direction, float height, float stunLength, bool m_ignoreGrounded) { throw new NotImplementedException(); }
        public virtual void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength, bool m_ignoreGrounded) { throw new NotImplementedException(); }
        #endregion

        #region Actions
        public virtual void UseAction(TimelineAsset attackTimeline)
        {
            StopPerformingAction();
            m_director.Play(attackTimeline);
            m_isPerformingAction = true;
        }

        public void OnActionEnd(PlayableDirector director)
        {
            m_isPerformingAction = false;
        }

        public void StopPerformingAction()
        {
            if (m_isPerformingAction)
            {
                m_director.Stop();
            }

            // Set attacking to false
            m_isPerformingAction = false;
        }
        #endregion

        #region Combat
        private float m_baseDamage;
        private ModifierType m_damageModifierType;
        private float m_damageModifier;

        public float actualDamage
        {
            get
            {
                return m_damageModifierType switch
                {
                    ModifierType.Additive => m_baseDamage + m_damageModifier,
                    ModifierType.Multiplicative => m_baseDamage * m_damageModifier,
                    _ => m_baseDamage,
                };
            }
        }

        public void ModifyDamage(ModifierType type, float modifier)
        {
            m_damageModifierType = type;
            m_damageModifier = modifier;
        }
        #endregion

        /* Attack Node Logic (OLD)
        #region NodeLogic
        private IEnumerator PlayAnimation(AnimationNode node)
        {
            // If there are issues with animator speed, check this first
            // init
            //m_anim.speed = node.Speed;
            //m_anim.Play(node.AnimationStateName);

            // running
            yield return new WaitForSeconds(node.Time);

            // exit
            //m_anim.speed = 1;

            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
        }
        private IEnumerator ApproachTarget(ApproachTargetNode node)
        {
            // init
            Vector3 targetPosition = m_targetTransform.position;

            // running
            //BeginGoToPosition(targetPosition);
            //SetMovementSpeed(node.Speed);
            bool withinRange = false;
            while (!withinRange)
            {
                // If not using initial position, update position
                if (!node.UseInitialPosition)
                {
                    targetPosition = m_targetTransform.position;
                    //BeginGoToPosition(targetPosition);
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
                MovePosition(Vector3.Lerp(startPosition, endPosition, t));

                Vector3 currentPos = transform.position;
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
                MovePosition(Vector3.Lerp(startPosition, endPosition, t));

                Vector3 currentPos = transform.position;
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
                MovePosition(target);

                Vector3 currentPos = transform.position;
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
                MovePosition(target);

                Vector3 currentPos = transform.position;
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
        */
    }
}
