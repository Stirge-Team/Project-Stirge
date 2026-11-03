using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Stirge.Combat
{
    using Hitboxes;
    using Stirge.UtilityAI.Statuses;
    using UtilityAI;

    public enum ModifierType
    {
        Additive,
        Multiplicative
    }
    
    public abstract class CombatEntity : Hittable
    {
        [Header("References")]
        [SerializeField] private CombatEntityMotor m_motor;
        [SerializeField] private EntityHealth m_health;
        [SerializeField] private PlayableDirector m_director;

        // action fields
        private Action m_currentAction;
        protected bool m_isPerformingAction;

        // status fields
        protected List<Status> m_inflictedStatuses = new();
        protected bool m_isStunned;

        private float m_stunCountdown;

        // combat fields
        private float m_baseDamage;
        private ModifierType m_damageModifierType;
        private float m_damageModifier;

        // reference properties
        public CombatEntityMotor Motor => m_motor;
        public EntityHealth Health => m_health;

        // public properties
        public bool IsStunned => m_isStunned;
        public bool IsPerformingAction => m_isPerformingAction;
        public float ActualDamage
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

        #region UnityEvents
        protected void Awake()
        {
            AwakeThis();
        }

        protected void Start()
        {
            StartThis();
        }

        protected void Update()
        {
            // update stun timer
            if (m_stunCountdown > 0f)
            {
                m_stunCountdown -= Time.deltaTime;

                if (m_stunCountdown <= 0f)
                {
                    StopStun();
                }
            }
            
            UpdateThis();

            UpdateStatuses();
        }

        protected void OnEnable()
        {
            m_director.stopped += OnActionEnd;
        }
        protected void OnDisable()
        {
            m_director.stopped -= OnActionEnd;
        }

        protected virtual void AwakeThis() { }
        protected virtual void StartThis() { }
        protected virtual void UpdateThis() { }
        #endregion

        #region Death State
        public void TakeDamage(int damage)
        {
            m_health.ModifyHealth(-Mathf.Abs(damage));
            OnDamageTaken(damage);
            if (m_health._isDead)
            {
                OnDeath();
            }
        }
        protected virtual void OnDamageTaken(int damage) { }
        protected virtual void OnDeath() { }

        public bool IsDead()
        {
            return m_health._isDead;
        }
        #endregion

        #region Status
        public void InflictStatus(CombatEntity inflicter, Status newStatus)
        {
            // Instant statuses don't get added to the list of Inflicted Statuses,
            // so there's no reason to do any checks, just apply the effect
            if (newStatus.DurationType == StatusDurationType.Instant)
            {
                newStatus.OnApply(inflicter, this);
                return;
            }
            
            Type statusType = newStatus.StatusType;
            int indexOfExistingStatus;
            // if Status is a Stun status, then 
            if (statusType.IsSubclassOf(typeof(Stun)))
            {
                indexOfExistingStatus = GetFirstIndexOfStatusDerived(statusType);
            }
            else
            {
                indexOfExistingStatus = GetIndexOfStatus(statusType, newStatus.DisplayName);
            }

            // If a Status of the same type already exists
            if (indexOfExistingStatus != -1)
            {
                switch (newStatus.StackType)
                {
                    case StatusStackType.Stackable:
                        // combine stacks
                        Status existingStackableStatus = m_inflictedStatuses[indexOfExistingStatus];
                        existingStackableStatus.AddStacks(newStatus.CurrentStacks);
                        return; // don't add
                    case StatusStackType.Single:
                        ClearStatus(indexOfExistingStatus);
                        break;
                    case StatusStackType.Unique:
                        return; // don't add
                    default:
                        break;
                }
            }

            // add the new Status
            m_inflictedStatuses.Add(newStatus);
            newStatus.OnApply(inflicter, this);
        }

        private void UpdateStatuses()
        {
            List<int> indicesToRemove = new();
            int index = 0;
            foreach (Status status in m_inflictedStatuses)
            {
                status.Update(this);

                if (status.ShouldThisClear(this))
                {
                    status.OnClear(this);
                    indicesToRemove.Add(index);
                }
                index++;
            }

            // remove backwards to avoid indicies from changing before removal
            for (int i = indicesToRemove.Count - 1; i >= 0; i--)
            {
                m_inflictedStatuses.RemoveAt(indicesToRemove[i]);
            }
        }

        /// <summary>
        /// Forcefully clear a <see cref="Status"/> ignoring any of its own clear logic.
        /// </summary>
        /// <param name="statusIndex"></param>
        public void ClearStatus(int statusIndex)
        {
            Status toRemove = m_inflictedStatuses[statusIndex];
            toRemove.OnClear(this);
            m_inflictedStatuses.RemoveAt(statusIndex);
        }

        /// <summary>
        /// Returns -1 if no <see cref="Status"/> of Type <paramref name="statusType"/> is inflicted.
        /// </summary>
        /// <returns>Index of first inflicted <see cref="Status"/> with matching type.</returns>
        public int GetIndexOfStatus(Type statusType, string statusName)
        {
            return m_inflictedStatuses.FindIndex(s => s.StatusType == statusType && s.DisplayName == statusName);
        }
        /// <summary>
        /// Returns -1 if no <see cref="Status"/> with name <paramref name="statusName"/> is inflicted.
        /// </summary>
        /// <returns>Index of first inflicted <see cref="Status"/> with matching <see cref="Status.DisplayName"/>.</returns>
        public int GetIndexOfStatus(string statusName)
        {
            return m_inflictedStatuses.FindIndex(status => status.DisplayName == statusName);
        }
        /// <summary>
        /// Returns -1 if no <see cref="Status"/> of Derived <see cref="Type"/> <paramref name="statusType"/> is inflicted.<br/>
        /// If an inflicted <see cref="Status"/> *is* <see cref="Type"/> <paramref name="statusType"/>, it will not be counted.
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>Index of first inflict <see cref="Status"/> of Derived <see cref="Type"/>.</returns>
        public int GetFirstIndexOfStatusDerived(Type statusType)
        {
            return m_inflictedStatuses.FindIndex(s => s.StatusType.IsSubclassOf(statusType));
        }

        /// <summary>
        /// Returns the number of the provided <paramref name="statusType"/> the Enemy is inflicted with.
        /// </summary>
        public int GetNumberOfInflictedStatus(Type statusType)
        {
            int count = 0;
            foreach (Status s in m_inflictedStatuses)
            {
                if (s.StatusType == statusType)
                    count++;
            }
            return count;
            //return m_inflictedStatuses.FindAll(status => status.StatusType == statusType).Count;
        }

        /// <summary>
        /// Used by the <see cref="AttackTimeline.StatusTrack.StatusTrack"/> and <see cref="HitboxCollider"/> to get references
        /// to <see cref="Status"/>es from the <see cref="CombatEntity"/>'s current <see cref="Action"/>.<br/>
        /// 
        /// There's probably a better way to do this but I couldn't think of anything in the time I had available.
        /// </summary>
        /// <param name="statusIndex"></param>
        /// <returns></returns>
        public Status CreateRuntimeStatusFromCurrentActionAtIndex(int statusIndex)
        {
            return m_currentAction.CreateRuntimeStatusAtIndex(statusIndex);
        }

        #region Stun
        public void StartStun(float duration)
        {
            if (duration <= 0f)
            {
                Debug.LogWarning($"Stun has an invalid duration of '{duration}' seconds!", this);
                return;
            }
            
            m_stunCountdown = duration;

            m_isStunned = true;
            OnStartStun(duration);
        }
        protected virtual void OnStartStun(float duration) { }
        public void StopStun()
        {
            if (m_isStunned)
            {
                m_isStunned = false;
                m_stunCountdown = 0f;
                OnStopStun();
            }
        }
        protected virtual void OnStopStun() { }
        #endregion

        #endregion

        #region Actions
        public virtual void PerformAction(Action action)
        {
            StopPerformingAction();
            m_currentAction = action;
            m_isPerformingAction = true;

            if (action.Timeline != null)
                m_director.Play(action.Timeline);
            m_motor.OnActionStart();
        }

        /// <summary>
        /// This should be added as a callback to m_director.stopped./>.
        /// </summary>
        /// <param name="director"></param>
        public void OnActionEnd(PlayableDirector director)
        {
            m_currentAction = null;
            m_isPerformingAction = false;
            m_motor.OnActionEnd();
            OnActionEndInternal(director);
        }
        protected virtual void OnActionEndInternal(PlayableDirector director) { }

        public void StopPerformingAction()
        {
            if (m_isPerformingAction)
            {
                m_director.Stop(); // OnActionEnd()
            }
        }
        #endregion

        #region Combat
        public void SetDamageModifier(ModifierType type, float modifier)
        {
            m_damageModifierType = type;
            m_damageModifier += modifier;
        }
        public void ResetDamgeModifier(float modifier)
        {
            m_damageModifier -= modifier;
        }

        /// <inheritdoc/>
        public override void OnHit(CombatEntity attacker, SerializedHitboxData hitboxData, Vector3 hitPosition, Vector3 hitVelocity, int[] statusesToApplyIndices, StatusInflictData[] statusInflictDatas)
        {
            TakeDamage(hitboxData.Damage);
            if (statusesToApplyIndices != null)
            {
                for (int i = 0, count = statusesToApplyIndices.Length; i < count; i++)
                {
                    int statusIndex = statusesToApplyIndices[i];
                    Status toInflict = CreateRuntimeStatusFromCurrentActionAtIndex(statusIndex);
                    toInflict.Setup(statusInflictDatas[i]);
                    InflictStatus(attacker, toInflict);
                }
            }
        }

        #endregion
    }
}
