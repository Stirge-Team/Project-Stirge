using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stirge.Combat
{
    using Attacks;
    using FrameFighter2.Hitbox;

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
        }
        private void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateThis(deltaTime);

            UpdateStatuses(deltaTime);

            if (m_isAttacking)
                UpdateAttacking();
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
        public void InflictStatus(Status status, Transform hitboxTransform, CombatEntity attackingEntity)
        {
            // inflict the Status
            if (attackingEntity == null)
                status.OnInflict(hitboxTransform, this);
            else
                status.OnInflict(hitboxTransform, this, attackingEntity);   
        }
        public void InflictTimedStatus(TimedStatus status, Transform hitboxTransform, CombatEntity attackingEntity)
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
                        newStun.OnInflict(hitboxTransform, this);
                    else
                        newStun.OnInflict(hitboxTransform, this, attackingEntity);

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
        public abstract void EnterKnockback(float strength, Vector3 direction, float height, float stunLength);
        public abstract void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength);
        #endregion

        #region Attacks
        // List of all the AttackNodes in the attack being used
        private AttackNode[] m_attackSequence;
        // Reference to the AttackNode currently being processed. Set to null once it is finished being processed
        private AttackNode m_currentAttackNode;
        // Stores the index of the AttackNode in m_attackSequence currently being processed.
        // After an AttackNode is finished processing, incremented by one to determine the next node or if the attack
        // is finished
        private int m_currentAttackIndex;
        // List of all the currently active Coroutines performing attack logic
        private Coroutine[] m_attackCoroutines;

        public virtual void UseAttack(AttackData attackData)
        {
            StopAttacking();
            m_attackSequence = attackData.EvaluateSequence();
            m_currentAttackNode = null;
            m_currentAttackIndex = -1;
            m_isAttacking = true;
        }

        private void UpdateAttacking()
        {
            // if no node is currently being processed
            if (m_currentAttackNode == null)
            {
                m_currentAttackIndex++;

                // if reached the end of the sequence, exit this state
                if (m_currentAttackIndex >= m_attackSequence.Length)
                {
                    m_attackCoroutines = null;
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
                // Clear Coroutines
                if (m_attackCoroutines != null)
                {
                    foreach (Coroutine coroutine in m_attackCoroutines)
                    {
                        if (coroutine != null)
                            StopCoroutine(coroutine);
                    }
                    m_attackCoroutines = null;
                }

                // Get array of all Attack Nodes to process this step
                AttackNode[] currentlyActiveNodes;
                if (m_currentAttackNode is SimultaneousAttackNode simultaneousAttackNode)
                {
                    int length = simultaneousAttackNode.Nodes.Length;
                    currentlyActiveNodes = new AttackNode[length];
                    System.Array.Copy(simultaneousAttackNode.Nodes, currentlyActiveNodes, length);
                }
                else
                {
                    currentlyActiveNodes = new AttackNode[1] { m_currentAttackNode };
                }

                StopAttackNodes(currentlyActiveNodes);
            }

            // Set attacking to false
            m_isAttacking = false;
        }

        private void StopAttackNodes(AttackNode[] attackNodes)
        {
            foreach (AttackNode node in attackNodes)
            {
                if (node == null)
                    continue;

                switch (node.GetType().Name)
                {
                    case nameof(AnimationNode):
                        // reset animator component
                        m_anim.speed = 1;
                        //m_anim.StopPlayback();

                        // apply motion from animation
                        AnimationNode animationNode = node as AnimationNode;
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

        private void StartAttackCoroutine()
        {
            void SetAttackCoroutineElement(AttackNode node, int index)
            {
                m_attackCoroutines[index] = node.GetType().Name switch
                {
                    nameof(AnimationNode) => StartCoroutine(PlayAnimation(node as AnimationNode)),
                    nameof(ApproachTargetNode) => StartCoroutine(ApproachTarget(node as ApproachTargetNode)),
                    nameof(TranslateNode) => StartCoroutine(Translate(node as TranslateNode)),
                    nameof(DelayNode) => StartCoroutine(Delay(node as DelayNode)),
                    nameof(TimedMoveNode) => StartCoroutine(TimedMove(node as TimedMoveNode)),
                    nameof(CurveMoveNode) => StartCoroutine(CurveMove(node as CurveMoveNode)),
                    nameof(SpeedMoveNode) => StartCoroutine(SpeedMove(node as SpeedMoveNode)),
                    nameof(AccelerateMoveNode) => StartCoroutine(AccelerateMove(node as AccelerateMoveNode)),
                    _ => null
                };
            }
            
            if (s_debug) Debug.Log($"Beginning processing {m_currentAttackNode.GetType().Name}.");

            if (m_currentAttackNode is SimultaneousAttackNode simultaneousAttackNode)
            {
                int nodeCount = simultaneousAttackNode.Nodes.Length;
                m_attackCoroutines = new Coroutine[nodeCount];
                for (int i = 0; i < nodeCount; i++)
                {
                    SetAttackCoroutineElement(simultaneousAttackNode.Nodes[i], i);
                }
            }
            else
            {
                m_attackCoroutines = new Coroutine[1];
                SetAttackCoroutineElement(m_currentAttackNode, 0);
            }

            // If all the AttackCoroutines are already null, then the attack is invalid
            if (m_attackCoroutines.All(c => c == null))
            {
                m_currentAttackNode = null;
                m_attackCoroutines = null;
                if (s_debug) Debug.Log($"None of the provided AttackNodes have implemented functionality. Stopping processing.");
                // Then reattempt to Update
                UpdateAttacking();
            }
        }

        private void OnAttackCoroutineFinished(AttackNode node)
        {
            if (m_attackCoroutines != null)
            {
                int nodeIndex;
                // set the coroutine of the Node that just finished to null
                // for non-Simultaneous Attack Nodes, the index will always be 0
                if (m_currentAttackNode is SimultaneousAttackNode simultaneousAttackNode)
                {
                    nodeIndex = System.Array.IndexOf(simultaneousAttackNode.Nodes, node);

                    // if the index of this node is equal to the SignificantAttackNodeIndex of this SimultaneousAttackNode,
                    // then this attack should be marked completed
                    if (nodeIndex == simultaneousAttackNode.SignificantAttackNodeIndex)
                    {
                        // Get all the currently active nodes
                        int length = simultaneousAttackNode.Nodes.Length;
                        AttackNode[] currentlyActiveNodes = new AttackNode[length];
                        System.Array.Copy(simultaneousAttackNode.Nodes, currentlyActiveNodes, length);
                        for (int i = 0; i < length; i++)
                        {
                            // If it is the significant AttackNode, clear
                            if (currentlyActiveNodes[i] == node)
                            {
                                currentlyActiveNodes[i] = null;
                            }
                            // If the AttackNode has already finished processing, clear
                            else if (m_attackCoroutines[i] == null)
                            {
                                currentlyActiveNodes[i] = null;
                            }
                        }

                        StopAttackNodes(currentlyActiveNodes);

                        m_currentAttackNode = null;
                        m_attackCoroutines = null;
                        if (s_debug) Debug.Log($"Finished processing Simultaneous Attack Node.");
                        return;
                    }
                }
                else
                {
                    nodeIndex = 0;
                }

                m_attackCoroutines[nodeIndex] = null;

                // If all the Coroutines are marked finished/All AttackNodes are finished processing,
                // Then mark the Current Attack Node as finished
                if (m_attackCoroutines.All(coroutine => coroutine == null))
                {
                    if (s_debug) Debug.Log($"Finished processing {m_currentAttackNode.GetType().Name}.");
                    m_currentAttackNode = null;
                    m_attackCoroutines = null;
                }
            }
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
            if (node.HasRootMotion)
                ApplyRootMotion();

            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
            OnAttackCoroutineFinished(node);
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
            OnAttackCoroutineFinished(node);
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
            OnAttackCoroutineFinished(node);
        }
        private IEnumerator Delay(DelayNode node)
        {
            // init

            // running
            yield return new WaitForSeconds(node.Delay);

            // exit
            if (s_debug) Debug.Log($"Finished processing {node.GetType().Name}.");
            OnAttackCoroutineFinished(node);
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
            OnAttackCoroutineFinished(node);
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
            OnAttackCoroutineFinished(node);
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
            OnAttackCoroutineFinished(node);
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
            OnAttackCoroutineFinished(node);
        }
        #endregion

        #endregion
    }
}
