using UnityEngine;
using System;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;
    using System.Linq;

    /// <summary>
    /// Used to determine how a <see cref="Status"/> handles Stacking.
    /// </summary>
    public enum StatusStackType
    {
        /// <summary>
        /// When additional copies of a <see cref="Status"/> are added, they combine their stacks together and leave one copy on the target.
        /// </summary>
        Stackable,
        /// <summary>
        /// When additional copies of a <see cref="Status"/> are added, they are added as an entirely separate entry.
        /// </summary>
        Unstackable,
        /// <summary>
        /// Additional copies of a <see cref="Status"/> cannot be added.
        /// </summary>
        Unique,
    }

    /// <summary>
    /// Determines if and for how long a <see cref="Status"/> lasts on a <see cref="CombatEntity"/>.
    /// </summary>
    public enum StatusDurationType
    {
        /// <summary>
        /// The <see cref="Status"/> applies its initial <see cref="Status.OnApply"/> effect and nothing else.
        /// </summary>
        Instant,
        /// <summary>
        /// The <see cref="Status"/> lasts for a length of time in seconds equal to <see cref="Status.Duration"/>.
        /// </summary>
        Timed,
        /// <summary>
        /// The <see cref="Status"/> lasts until <see cref="Status.ShouldThisClear"/> returns true.
        /// </summary>
        Conditional
    }

    /// <summary>
    /// Part of Attack <see cref="Action"/>s. They are applied to <see cref="CombatEntity"/> objects when certain  conditions are met.<br/>
    /// When inheriting this class, you must also implement either <see cref="INotSetupable"/> or one of the <see cref="ISetupable{TArg}"/> interfaces.
    /// The number of generic arguments in <see cref="ISetupable{TArg}"/> should be equal to the number of unique properties you want your <see cref="Status"/> to have.
    /// </summary>
    public abstract class Status : IScorable
    {
        /// <summary>
        /// The <see cref="CombatEntity"/> who applied this <see cref="Status"/>.
        /// </summary>
        private CombatEntity m_user;

        // fields
        /// <summary>
        /// The returned score of this <see cref="Status"/> will be multiplied by this value.
        /// </summary>
        protected float m_scoreScaling;
        /// <summary>
        /// The name of this <see cref="Status"/> for testing purposes and in case we want to be able to show the Player names of Statuses.
        /// </summary>
        protected string m_displayName;
        /// <summary>
        /// Who this <see cref="Status"/> is targeting in an Attack.
        /// </summary>
        protected EntityTargetType m_target;
        /// <summary>
        /// How this <see cref="Status"/> handles Stacking.
        /// </summary>
        protected StatusStackType m_stackType;
        /// <summary>
        /// For <see cref="Status"/>es of <see cref="StatusStackType.Stackable"/>, the max number of Stacks that can be applied for this type of <see cref="Status"/>.
        /// </summary>
        protected int m_maxStacks;
        /// <summary>
        /// How long this <see cref="Status"/> lasts for when inflicted.
        /// </summary>
        protected StatusDurationType m_durationType;
        /// <summary>
        /// For <see cref="Status"/> of <see cref="StatusDurationType.Timed"/>, the length of time in seconds it lasts for.
        /// </summary>
        protected float m_duration;
        /// <summary>
        /// The Conditions that must be met for this <see cref="Status"/> to be Inflicted on its target.
        /// </summary>
        protected ICondition[] m_conditions;
        /// <summary>
        /// The ScoringMethods that determine the score this <see cref="Status"/> will return when Evaluated as part of a <see cref="UtilityBrain"/> Evaluating the <see cref="Action"/> it is part of.
        /// </summary>
        protected IScoringMethod[] m_scoringMethods;

        // variables
        protected int m_currentStacks;
        protected float m_durationCountdown;

        // properties
        protected CombatEntity user => m_user;
        public float ScoreScaling => m_scoreScaling;
        public string DisplayName => m_displayName;
        public EntityTargetType Target => m_target;
        public StatusStackType StackType => m_stackType;
        public int MaxStacks => m_maxStacks;
        public StatusDurationType DurationType => m_durationType;
        public float Duration => m_duration;

        public int CurrentStacks => m_currentStacks;
        public float DurationCountdown => m_durationCountdown;

        public abstract Type StatusType { get; }

        /// <inheritdoc/>
        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float baseScore = (EvaluateInternal(user, target) + m_scoringMethods.Sum(s => s.Evaluate(user, target))) / (m_scoringMethods.Length + 1);
            return baseScore * m_scoreScaling;
        }

        /// <inheritdoc cref="IScorable.Evaluate"/>
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        /// <summary>
        /// What happens when this <see cref="Status"/> is first Inflicted onto <paramref name="target"/>.<br/>
        /// For non-Instant <see cref="Status"/>es, this is where a reference to <paramref name="user"/> is stored in <see cref="m_user"/>.
        /// </summary>
        public void OnApply(CombatEntity user, CombatEntity target)
        {
            if (m_durationType != StatusDurationType.Instant)
                m_user = user;
            OnApplyInternal(user, target);
        }
        /// <summary>
        /// Override to provide your <see cref="Status"/>es logic for what happens when the <see cref="Status"/> is initially inflicted.
        /// </summary>
        protected abstract void OnApplyInternal(CombatEntity user, CombatEntity target);

        /// <summary>
        /// What happens each frame while this <see cref="Status"/> is inflicted.
        /// </summary>
        public void Update(CombatEntity target)
        {
            if (m_durationCountdown < m_duration)
                m_durationCountdown += Time.deltaTime;
            UpdateInternal(target);
        }
        /// <summary>
        /// Override to provide your <see cref="Status"/>es logic for what happens each frame while the <see cref="Status"/> is inflicted.
        /// </summary>
        protected abstract void UpdateInternal(CombatEntity target);

        /// <summary>
        /// What happens when this <see cref="Status"/> is removed from the <paramref name="target"/>.
        /// </summary>
        public void OnClear(CombatEntity target)
        {
            m_currentStacks = 0;
            m_durationCountdown = 0f;
            OnClearInternal(target);
        }
        /// <summary>
        /// Override to provide your <see cref="Status"/>es logic for what happens when the <see cref="Status"/> is removed from the <paramref name="target"/>..
        /// </summary>
        protected abstract void OnClearInternal(CombatEntity target);

        /// <summary>
        /// If your <see cref="Status"/> is <see cref="StatusDurationType.Conditional"/>, override this method to determine when your <see cref="Status"/> should end.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool ShouldThisClear(CombatEntity target)
        {
            return m_durationType switch
            {
                StatusDurationType.Instant => true,
                StatusDurationType.Timed => m_durationCountdown >= m_duration,
                _ => false,
            };
        }

        public void AddStacks(int stackCount)
        {
            m_currentStacks = Math.Min(m_currentStacks + stackCount, m_maxStacks);
        }

        #region Setup
        public void Setup(ICondition[] conditions, IScoringMethod[] scoringMethods)
        {
            m_conditions = conditions;
            m_scoringMethods = scoringMethods;
        }

        private static TStatus CreateInternal<TStatus>(float scoreScaling, string displayName, StatusStackType stackType, int maxStacks, StatusDurationType durationType, float duration) where TStatus : Status, new()
        {
            var status = new TStatus()
            {
                m_scoreScaling = scoreScaling,
                m_displayName = displayName,
                m_stackType = stackType,
                m_maxStacks = maxStacks,
                m_durationType = durationType,
                m_duration = duration
            };
            return status;
        }
        public static TStatus Create<TStatus>(float scoreScaling, string displayName, StatusStackType stackType, int maxStacks, StatusDurationType durationType, float duration) where TStatus : Status, INotSetupable, new()
        {
            return CreateInternal<TStatus>(scoreScaling, displayName, stackType, maxStacks, durationType, duration);
        }
        public static TStatus Create<TStatus, TArg>(TArg arg, float scoreScaling, string displayName, StatusStackType stackType, int maxStacks, StatusDurationType durationType, float duration) where TStatus : Status, ISetupable<TArg>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, displayName, stackType, maxStacks, durationType, duration);
            status.Setup(arg);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ0>(TArg0 arg0, Targ0 arg1, float scoreScaling, string displayName, StatusStackType stackType, int maxStacks, StatusDurationType durationType, float duration) where TStatus : Status, ISetupable<TArg0, Targ0>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, displayName, stackType, maxStacks, durationType, duration);
            status.Setup(arg0, arg1);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling, string displayName, StatusStackType stackType, int maxStacks, StatusDurationType durationType, float duration) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, displayName, stackType, maxStacks, durationType, duration);
            status.Setup(arg0, arg1, arg2);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling, string displayName, StatusStackType stackType, int maxStacks, StatusDurationType durationType, float duration) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, displayName, stackType, maxStacks, durationType, duration);
            status.Setup(arg0, arg1, arg2, arg3);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling, string displayName, StatusStackType stackType, int maxStacks, StatusDurationType durationType, float duration) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, displayName, stackType, maxStacks, durationType, duration);
            status.Setup(arg0, arg1, arg2, arg3, arg4);
            return status;
        }
        #endregion
    }
}
