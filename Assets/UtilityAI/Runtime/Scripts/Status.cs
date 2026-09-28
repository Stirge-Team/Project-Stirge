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

    public abstract class Status : IScorable
    {
        private CombatEntity m_user;
        
        // fields
        protected float m_scoreScaling;
        protected string m_displayName;
        protected EntityTargetType m_target;
        protected StatusStackType m_stackType;
        protected int m_maxStacks;
        protected StatusDurationType m_durationType;
        protected float m_duration;
        protected ICondition[] m_conditions;
        protected IScoringMethod<Status>[] m_scoringMethods;

        // variables
        protected int m_currentStacks;
        protected float m_timer;

        // properties
        protected CombatEntity user => m_user;
        public float ScoreScaling => m_scoreScaling;
        public string DisplayName => m_displayName;
        public EntityTargetType Target => m_target;
        public StatusStackType StackType => m_stackType;
        public int MaxStacks => m_maxStacks;
        public StatusDurationType DurationType => m_durationType;
        public float Duration => m_duration;
        public ICondition[] Conditions => m_conditions;
        public IScoringMethod<Status>[] ScoringMethods => m_scoringMethods;

        public int CurrentStacks => m_currentStacks;
        public float Timer => m_timer;

        public abstract Type StatusType { get; }

        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            float baseScore = (EvaluateInternal(user, target) + m_scoringMethods.Sum(s => s.Evaluate(user, target))) / (m_scoringMethods.Length + 1);
            return baseScore * m_scoreScaling;
        }
        protected abstract float EvaluateInternal(UtilityEnemy user, CombatEntity target);

        public void OnApply(CombatEntity user, CombatEntity target)
        {
            m_user = user;
            OnApplyInternal(target);
        }
        protected abstract void OnApplyInternal(CombatEntity target);

        public void Update(CombatEntity target)
        {
            m_timer += Time.deltaTime;
            UpdateInternal(target);
        }
        protected abstract void UpdateInternal(CombatEntity target);

        public void OnClear(CombatEntity target)
        {
            m_currentStacks = 0;
            m_timer = 0f;
            OnClearInternal(target);
        }
        protected abstract void OnClearInternal(CombatEntity target);

        public virtual bool ShouldThisClear(CombatEntity target)
        {
            return m_durationType switch
            {
                StatusDurationType.Instant => true,
                StatusDurationType.Timed => m_timer >= m_duration,
                _ => false,
            };
        }

        public void AddStacks(int stackCount)
        {
            m_currentStacks = Math.Min(m_currentStacks + stackCount, m_maxStacks);
        }

        #region Setup
        public void Setup(ICondition[] conditions, IScoringMethod<Status>[] scoringMethods)
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
