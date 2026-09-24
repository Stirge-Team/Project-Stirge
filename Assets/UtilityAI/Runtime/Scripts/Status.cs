using UnityEngine;
using System;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;
    using System.Linq;

    public enum StatusStackType
    {
        Stackable,
        Unstackable,
        Unique,
    }
    public enum StatusDurationType
    {
        Instant,
        Timed,
        Conditional
    }

    public abstract class Status
    {
        private CombatEntity m_user;
        
        // fields
        protected float m_scoreScaling;
        protected StatusStackType m_stackType;
        protected StatusDurationType m_durationType;
        protected string m_displayName;
        protected int m_maxStacks;
        protected ICondition[] m_conditions;
        protected ScoringMethod[] m_scoringMethods;

        // variables
        protected int m_currentStackCount;

        // properties
        public float scoreScaling => m_scoreScaling;
        public StatusStackType stackType => m_stackType;
        public StatusDurationType durationType => m_durationType;
        public string displayName => m_displayName;
        public int maxStacks => m_maxStacks;
        public ICondition[] conditions => m_conditions;
        public ScoringMethod[] scoringMethods => m_scoringMethods;

        public int currentStackCount
        {
            get => m_currentStackCount;
            set => m_currentStackCount = value;
        }

        public abstract Type statusType { get; }

        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float baseScore = (EvaluateInternal(user, target) + m_scoringMethods.Sum(s => s.Evaluate(user, target))) / (m_scoringMethods.Length + 1);
            return baseScore * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        /// <summary>
        /// User is passed here so it may be saved as a reference for any effects that require the applier of the effect during Resolve or Clear.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns>If the Status should end. This should be false for any non-instant Statuses.</returns>
        public abstract bool OnApply(CombatEntity user, CombatEntity target);
        /// <summary>
        /// Update method.
        /// </summary>
        /// <param name="target"></param>
        public virtual void Update(CombatEntity target) { }
        /// <summary>
        /// Run before removing the Status from the Statuses array.
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnClear(CombatEntity target) { }

        /// <returns>If the Status should end. You should return 'false' unless the <see cref="durationType"/> of this Status is <see cref="StatusDurationType.Conditional"/>.</returns>
        public virtual bool ShouldThisClear(CombatEntity target) { return false; }

        #region Setup
        private static TStatus CreateInternal<TStatus>(float scoreScaling, StatusStackType stackType, StatusDurationType durationType, string displayName, int maxStacks, ICondition[] conditions, ScoringMethod[] scoringMethods) where TStatus : Status, new()
        {
            var status = new TStatus()
            {
                m_scoreScaling = scoreScaling,
                m_stackType = stackType,
                m_durationType = durationType,
                m_displayName = displayName,
                m_maxStacks = maxStacks,
                m_conditions = conditions,
                m_scoringMethods = scoringMethods
            };
            return status;
        }
        public static TStatus Create<TStatus>(float scoreScaling, StatusStackType stackType, StatusDurationType durationType, string displayName, int maxStacks, ICondition[] conditions, ScoringMethod[] scoringMethods) where TStatus : Status, INotSetupable, new()
        {
            return CreateInternal<TStatus>(scoreScaling, stackType, durationType, displayName, maxStacks, conditions, scoringMethods);
        }
        public static TStatus Create<TStatus, TArg>(TArg arg, float scoreScaling, StatusStackType stackType, StatusDurationType durationType, string displayName, int maxStacks, ICondition[] conditions, ScoringMethod[] scoringMethods) where TStatus : Status, ISetupable<TArg>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, stackType, durationType, displayName, maxStacks, conditions, scoringMethods);
            status.Setup(arg);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ0>(TArg0 arg0, Targ0 arg1, float scoreScaling, StatusStackType stackType, StatusDurationType durationType, string displayName, int maxStacks, ICondition[] conditions, ScoringMethod[] scoringMethods) where TStatus : Status, ISetupable<TArg0, Targ0>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, stackType, durationType, displayName, maxStacks, conditions, scoringMethods);
            status.Setup(arg0, arg1);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2>(TArg0 arg0, Targ1 arg1, Targ2 arg2, float scoreScaling, StatusStackType stackType, StatusDurationType durationType, string displayName, int maxStacks, ICondition[] conditions, ScoringMethod[] scoringMethods) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, stackType, durationType, displayName, maxStacks, conditions, scoringMethods);
            status.Setup(arg0, arg1, arg2);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, float scoreScaling, StatusStackType stackType, StatusDurationType durationType, string displayName, int maxStacks, ICondition[] conditions, ScoringMethod[] scoringMethods) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, stackType, durationType, displayName, maxStacks, conditions, scoringMethods);
            status.Setup(arg0, arg1, arg2, arg3);
            return status;
        }
        public static TStatus Create<TStatus, TArg0, Targ1, Targ2, TArg3, TArg4>(TArg0 arg0, Targ1 arg1, Targ2 arg2, TArg3 arg3, TArg4 arg4, float scoreScaling, StatusStackType stackType, StatusDurationType durationType, string displayName, int maxStacks, ICondition[] conditions, ScoringMethod[] scoringMethods) where TStatus : Status, ISetupable<TArg0, Targ1, Targ2, TArg3, TArg4>, new()
        {
            var status = CreateInternal<TStatus>(scoreScaling, stackType, durationType, displayName, maxStacks, conditions, scoringMethods);
            status.Setup(arg0, arg1, arg2, arg3, arg4);
            return status;
        }
        #endregion
    }
}
