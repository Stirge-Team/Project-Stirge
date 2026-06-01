using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stirge.Combat
{
    using Attacks;

    public abstract class CombatEntity : MonoBehaviour
    {
        private static bool s_debug = false;
        
        [Header("Components")]
        [SerializeField] protected Animator m_anim;

        [Header("Combat Properties")]
        [SerializeField] protected EntityHealth m_health;
        public EntityHealth Health => m_health;

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
                UpdateAttacking(deltaTime);
        }

        protected virtual void AwakeThis() { }
        protected virtual void UpdateThis(float deltaTime) { }
        #endregion

        #region Transformation
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
        public abstract Vector3 GetForward();

        protected abstract void BeginGoToPosition(Vector3 newPosition);
        protected abstract void StopGoToPosition();

        protected abstract float GetMovementSpeed();
        protected abstract void SetMovementSpeed(float speed);
        protected abstract void ResetMovementSpeed();
        #endregion

        #region Death State
        public void TakeDamage(int damage)
        {
            m_health.ModifyHealth(-Mathf.Abs(damage));
            OnDamageTaken(damage);
        }
        protected abstract void OnDamageTaken(int damage);

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
        public abstract void EnterKnockback(float strength, Vector3 direction, float height, float stunLength);
        public abstract void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength);
        #endregion

        #region Attacks
        private AttackNode[] m_attackSequence;
        private AttackNode m_currentAttackNode;
        private int m_currentAttackIndex;
        private Coroutine[] m_attackCoroutines;

        public void UseAttack(AttackData attackData)
        {
            StopAttacking();
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
                    m_currentAttackNode = null;
                    m_attackCoroutines = null;
                    if (s_debug) Debug.Log($"Finished processing Simultaneous Attack Node.");
                }
            }
        }

        private void StartAttackCoroutine()
        {
            if (s_debug) Debug.Log($"Beginning processing {m_currentAttackNode.GetType().Name}.");

            if (m_currentAttackNode is SimultaneousAttackNode simultaneousAttackNode)
            {
                m_attackCoroutines = new Coroutine[simultaneousAttackNode.Nodes.Length];
                int num = 0;
                foreach (AttackNode node in simultaneousAttackNode.Nodes)
                {
                    switch (node.GetType().Name)
                    {
                        case nameof(AnimationNode):
                            m_attackCoroutines[num] = StartCoroutine(PlayAnimation(node as AnimationNode));
                            break;
                        case nameof(ApproachTargetNode):
                            m_attackCoroutines[num] = StartCoroutine(ApproachTarget(node as ApproachTargetNode));
                            break;
                        case nameof(TranslateNode):
                            m_attackCoroutines[num] = StartCoroutine(Translate(node as TranslateNode));
                            break;
                        case nameof(DelayNode):
                            m_attackCoroutines[num] = StartCoroutine(Delay(node as DelayNode));
                            break;
                    }
                    num++;
                }
            }
            else
            {
                m_attackCoroutines = new Coroutine[1];
                switch (m_currentAttackNode.GetType().Name)
                {
                    case nameof(AnimationNode):
                        m_attackCoroutines[0] = StartCoroutine(PlayAnimation(m_currentAttackNode as AnimationNode));
                        break;
                    case nameof(ApproachTargetNode):
                        m_attackCoroutines[0] = StartCoroutine(ApproachTarget(m_currentAttackNode as ApproachTargetNode));
                        break;
                    case nameof(TranslateNode):
                        m_attackCoroutines[0] = StartCoroutine(Translate(m_currentAttackNode as TranslateNode));
                        break;
                    case nameof(DelayNode):
                        m_attackCoroutines[0] = StartCoroutine(Delay(m_currentAttackNode as DelayNode));
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

            if (s_debug) Debug.Log($"Finished processing Animation Node.");
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
            if (s_debug) Debug.Log($"Finished processing Approach Target node.");
            OnAttackCoroutineFinished(node);
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
            if (s_debug) Debug.Log($"Finished processing Translate Node.");
            OnAttackCoroutineFinished(node);
        }
        private IEnumerator Delay(DelayNode node)
        {
            // init

            // running
            yield return new WaitForSeconds(node.Delay);

            // exit
            if (s_debug) Debug.Log($"Finished processing Delay Node.");
            OnAttackCoroutineFinished(node);
        }
        #endregion
    }
}
