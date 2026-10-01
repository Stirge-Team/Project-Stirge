using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;

    public enum ActionType
    {
        Melee,
        Ranged,
        Movement
    }

    public class Action : IScorable
    {
        // fields
        private float m_scoreScaling = 1f;
        private float m_duration;
        private string m_displayName;
        private ActionType m_actionType;
        private TimelineAsset m_timeline;
        private float m_damage = 1f;
        private float m_range = 1f;
        private Status[] m_statuses;
        private ICondition[] m_conditions;
        private IScoringMethod[] m_scoringMethods;

        // properties
        public float duration => m_duration;
        public string displayName => m_displayName;
        public ActionType actionType => m_actionType;
        public float damage => m_damage;
        public float range => m_range;

        public float Evaluate(UtilityEnemy user, CombatEntity target)
        {
            if (!Enumerable.All(m_conditions, condition => condition.Evaluate(user, target)))
                return 0f;

            // Get score from Scoring Methods
            int scoringMethodCount = m_scoringMethods.Length;
            float scoringMethodScore = scoringMethodCount > 0 ? m_scoringMethods.Sum(m => m.Evaluate(user, target)) / scoringMethodCount : 0f;

            // Get Score from Statuses
            int statusCount = m_statuses.Length;
            float statusScore = statusCount > 0 ? m_statuses.Sum(s => s.Evaluate(user, target)) / statusCount : 0f;

            return (scoringMethodScore + statusScore + EvaluateInternal(user, target)) * m_scoreScaling;
        }
        protected virtual float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return 0f;
        }

        public virtual void Perform(CombatEntity user, CombatEntity target)
        {
            if (m_timeline != null)
                user.UseAction(m_timeline);
        }

        #region Setup
        public void Setup(Status[] statuses, ICondition[] conditions, IScoringMethod[] scoringMethods)
        {
            m_statuses = statuses;
            m_conditions = conditions;
            m_scoringMethods = scoringMethods;
        }

        public static TAction Create<TAction>(float scaling, float duration, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range) where TAction : Action, new()
        {
            TAction action = new()
            {
                m_scoreScaling = scaling,
                m_duration = duration,
                m_displayName = displayName,
                m_actionType = actionType,
                m_timeline = timeline,
                m_damage = damage,
                m_range = range
            };

            return action;
        }
        public static TAction Create<TAction, TArg>(TArg arg, float scaling, float duration, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range) where TAction : Action, ISetupable<TArg>, new()
        {
            var action = Create<TAction>(scaling, duration, displayName, actionType, timeline, damage, range);
            action.Setup(arg);
            return action;
        }
        public static TAction Create<TAction, TArg0, TArg1>(TArg0 arg0, TArg1 arg1, float scaling, float duration, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range) where TAction : Action, ISetupable<TArg0, TArg1>, new()
        {
            var action = Create<TAction>(scaling, duration, displayName, actionType, timeline, damage, range);
            action.Setup(arg0, arg1);
            return action;
        }
        public static TAction Create<TAction, TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2, float scaling, float duration, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range) where TAction : Action, ISetupable<TArg0, TArg1, TArg2>, new()
        {
            var action = Create<TAction>(scaling, duration, displayName, actionType, timeline, damage, range);
            action.Setup(arg0, arg1, arg2);
            return action;
        }
        public static TAction Create<TAction, TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, float scaling, float duration, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range) where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
        {
            var action = Create<TAction>(scaling, duration, displayName, actionType, timeline, damage, range);
            action.Setup(arg0, arg1, arg2, arg3);
            return action;
        }
        public static TAction Create<TAction, TArg0, TArg1, TArg2, TArg3, TArg4>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, float scaling, float duration, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range) where TAction : Action, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
        {
            var action = Create<TAction>(scaling, duration, displayName, actionType, timeline, damage, range);
            action.Setup(arg0, arg1, arg2, arg3, arg4);
            return action;
        }
        #endregion
    }
}
