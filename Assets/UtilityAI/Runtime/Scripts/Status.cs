using UnityEngine;
using System;
using System.Linq;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;
    using Stirge.Combat.Hitboxes;

    /// <summary>
    /// Used to determine the target of a <see cref="Status"/>.
    /// </summary>
    public enum StatusTarget
    {
        /// <summary>
        /// The <see cref="CombatEntity"/> that is using the <see cref="Action"/> the <see cref="Status"/> is apart of will be inflicted with the <see cref="Status"/>.
        /// </summary>
        User,
        /// <summary>
        /// The target of the <see cref="CombatEntity"/> that is using the <see cref="Action"/> the <see cref="Status"/> is apart of will be inflicted with the <see cref="Status"/>.
        /// </summary>
        Target
    }

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
        /// When an additional copy of a <see cref="Status"/> is added, it replaces the existing one.
        /// </summary>
        Single,
        /// <summary>
        /// Additional copies of a <see cref="Status"/> cannot be added and existing copies cannot be replaced.
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
        private CombatEntity m_inflicter;

        // fields
        /// <summary>
        /// The returned score of this <see cref="Status"/> will be multiplied by this value.
        /// </summary>
        private float m_scoreScaling;
        /// <summary>
        /// The name of this <see cref="Status"/> for testing purposes and in case we want to be able to show the Player names of Statuses.
        /// </summary>
        private string m_displayName;
        /// <summary>
        /// How this <see cref="Status"/> handles Stacking. Please note, the Public Property <see cref="StackType"/> used to access this value can be overriden in child-class definitions,
        /// meaning this value will not matter if so.
        /// </summary>
        private StatusStackType m_stackType;
        /// <summary>
        /// For <see cref="Status"/>es of <see cref="StatusStackType.Stackable"/>, the max number of Stacks that can be applied for this type of <see cref="Status"/>.
        /// </summary>
        protected int m_maxStacks;
        /// <summary>
        /// How long this <see cref="Status"/> lasts for when inflicted. Please note, the Public Property <see cref="DurationType"/> used to access this value can be overriden in child-class definitions,
        /// meaning this value will not matter if so.
        /// </summary>
        private StatusDurationType m_durationType;
        /// <summary>
        /// For <see cref="Status"/> of <see cref="StatusDurationType.Timed"/>, the length of time in seconds it lasts for.
        /// </summary>
        protected float m_duration;
        /// The Conditions that must be met for this <see cref="Status"/> to be Inflicted on its target.
        /// </summary>
        private ICondition[] m_conditions;
        /// <summary>
        /// The ScoringMethods that determine the score this <see cref="Status"/> will return when Evaluated as part of a <see cref="UtilityBrain"/> Evaluating the <see cref="Action"/> it is part of.
        /// </summary>
        private IScoringMethod[] m_scoringMethods;

        // variables
        protected int m_currentStacks;
        protected float m_countdown;

        // properties
        protected CombatEntity inflicter => m_inflicter;
        public float ScoreScaling => m_scoreScaling;
        public string DisplayName => m_displayName;
        /// <summary>
        /// Override to force a set value for <see cref="m_stackType"/>.
        /// </summary>
        public virtual StatusStackType StackType => m_stackType;
        public int MaxStacks => m_maxStacks;
        /// <summary>
        /// Override to force a set value for <see cref="m_durationType"/>.
        /// </summary>
        public virtual StatusDurationType DurationType => m_durationType;
        public float Duration => m_duration;

        public int CurrentStacks => m_currentStacks;
        public float DurationCountdown => m_countdown;

        public abstract Type StatusType { get; }

        /// <inheritdoc/>
        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            // If any Condition is false, return 0 score
            foreach (ICondition condition in m_conditions)
            {
                if (!condition.Evaluate(user, target))
                    return 0f;
            }

            float baseScore = (EvaluateInternal(user, target) + m_scoringMethods.Sum(s => s.Evaluate(user, target))) / (m_scoringMethods.Length + 1);
            return baseScore * m_scoreScaling;
        }

        /// <inheritdoc cref="IScorable.Evaluate"/>
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        /// <summary>
        /// What happens when this <see cref="Status"/> is first Inflicted onto <paramref name="target"/>.<br/>
        /// For non-Instant <see cref="Status"/>es, this is where a reference to <paramref name="inflicter"/> is stored in <see cref="m_inflicter"/>.
        /// </summary>
        public void OnApply(CombatEntity inflicter, CombatEntity target)
        {
            m_inflicter = inflicter;
            if (DurationType == StatusDurationType.Timed)
                m_countdown = m_duration;
            OnApplyInternal(inflicter, target);
        }
        /// <summary>
        /// Override to provide your <see cref="Status"/>es logic for what happens when the <see cref="Status"/> is initially inflicted.
        /// </summary>
        protected abstract void OnApplyInternal(CombatEntity inflicter, CombatEntity target);

        /// <summary>
        /// What happens each frame while this <see cref="Status"/> is inflicted.
        /// </summary>
        public void Update(CombatEntity target)
        {
            if (m_countdown > 0)
                m_countdown -= Time.deltaTime;
            UpdateInternal(target);
        }
        /// <summary>
        /// Override to provide your <see cref="Status"/>es logic for what happens each frame while the <see cref="Status"/> is inflicted.
        /// </summary>
        protected virtual void UpdateInternal(CombatEntity target) { }

        /// <summary>
        /// What happens when this <see cref="Status"/> is removed from the <paramref name="target"/>.
        /// </summary>
        public void OnClear(CombatEntity target)
        {
            m_currentStacks = 0;
            m_countdown = 0f;
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
            if (DurationType == StatusDurationType.Timed && m_countdown <= 0f)
                return true;
            return false;
        }

        public void AddStacks(int stackCount)
        {
            m_currentStacks = Math.Min(m_currentStacks + stackCount, m_maxStacks);
        }

        #region Setup
        /// <summary>
        /// As <see cref="Action"/>s may be destroyed while this <see cref="Status"/> is still active, so if you need any properties from the <see cref="Action"/> this <see cref="Status"/> is from, then retrive them here.
        /// </summary>
        /// <param name="action"></param>
        public virtual void Setup(Action action) { }

        /// <summary>
        /// When a <see cref="Status"/> is created, you may want to initialise some values based on those stored in a <see cref="StatusInflictData"/>.<br/>
        /// You can do that here.
        /// </summary>
        /// <param name="inflictData"></param>
        public void Setup(StatusInflictData inflictData)
        {
            if (StackType == StatusStackType.Stackable)
                AddStacks(inflictData.Stacks);
            if (DurationType == StatusDurationType.Timed)
                m_duration = inflictData.Duration;
        }

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
                m_maxStacks = (maxStacks == 0 ? int.MaxValue : maxStacks), // if maxStacks is 0, treat it as infinite
                m_durationType = durationType,
                m_duration = duration,
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
