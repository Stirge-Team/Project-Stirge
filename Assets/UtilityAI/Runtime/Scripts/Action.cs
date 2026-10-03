using System.Linq;
using UnityEngine.Timeline;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;
    using UnityEngine;

    public enum ActionType
    {
        Melee,
        Ranged,
        Movement
    }

    /// <summary>
    /// Part of the <see cref="UtilityBrain"/>. Used to determine the things the <see cref="UtilityEnemy"/> can do.<br/>
    /// When inheriting this class, you must also implement either <see cref="INotSetupable"/> or one of the <see cref="ISetupable{TArg}"/> interfaces.
    /// The number of generic arguments in <see cref="ISetupable{TArg}"/> should be equal to the number of unique properties you want your <see cref="Action"/> to have.
    /// </summary>
    public class Action : IScorable
    {
        // fields
        /// <summary>
        /// The returned score of this <see cref="Action"/> will be multiplied by this value.
        /// </summary>
        private float m_scoreScaling = 1f;
        /// <summary>
        /// The length of time in seconds the <see cref="UtilityBrain"/> will wait after this <see cref="Action"/> is Performed until it Evaluates a new <see cref="Action"/>.
        /// </summary>
        private float m_duration;
        /// <summary>
        /// The name of this <see cref="Action"/> for testing purposes and in case we want to be able to show the Player names of Attacks.
        /// </summary>
        private string m_displayName;
        /// <summary>
        /// Which type of <see cref="Action"/> this is.
        /// </summary>
        private ActionType m_actionType;
        /// <summary>
        /// The <see cref="TimelineAsset"/> that will be played when this <see cref="Action"/> is performed.
        /// </summary>
        private TimelineAsset m_timeline;
        /// <summary>
        /// The base damage if this is an Attack.
        /// </summary>
        private float m_damage = 1f;
        /// <summary>
        /// The base range if this is an Attack.
        /// </summary>
        private float m_range = 1f;
        /// <summary>
        /// The <see cref="Status"/>es that can be inflicted by this <see cref="Action"/>. See the individual <see cref="Status"/>es for targeting information.
        /// </summary>
        private Status[] m_statuses;
        /// <summary>
        /// The Conditions that must be met for this <see cref="Action"/> to be Performed.
        /// </summary>
        private ICondition[] m_conditions;
        /// <summary>
        /// The ScoringMethods that determine the score this <see cref="Action"/> will return when Evaluated.
        /// </summary>
        private IScoringMethod[] m_scoringMethods;

        private bool m_performed;
        private float m_lifeTime;

        private bool[] m_inflictedStatuses;

        // properties
        public float duration => m_duration;
        public string displayName => m_displayName;
        public ActionType actionType => m_actionType;
        public float damage => m_damage;
        public float range => m_range;

        /// <inheritdoc/>
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

        /// <inheritdoc cref="IScorable.Evaluate"/>
        protected virtual float EvaluateInternal(UtilityEnemy user, CombatEntity target)
        {
            return 0f;
        }

        /// <summary>
        /// What happens when the <paramref name="user"/> Performs this <see cref="Action"/>.
        /// </summary>
        public void Perform(CombatEntity user, CombatEntity target)
        {
            if (!m_performed)
            {
                m_performed = true;
                
                if (m_timeline != null)
                    user.UseAction(m_timeline);
            }

            // Update Statuses
            for (int i = 0, count = m_statuses.Length; i < count; i++)
            {
                Status status = m_statuses[i];

                if (status.InflictCondition == StatusInflictCondition.AfterDelay && // If this Status is a Delay status
                    !m_inflictedStatuses[i] && // If this Status has not yet been inflicted
                    m_lifeTime >= status.InflictDelay) // If the required amount of time has passed
                {
                    status.Inflict(user, target);
                    m_inflictedStatuses[i] = true;
                }
            }

            m_lifeTime += Time.deltaTime;

            PerformInternal(user, target);
        }
        protected virtual void PerformInternal(CombatEntity user, CombatEntity target) { }

        public virtual void Reset()
        {
            m_performed = false;
            m_lifeTime = 0f;
        }

        #region Setup
        public void Setup(Status[] statuses, ICondition[] conditions, IScoringMethod[] scoringMethods)
        {
            m_statuses = statuses;
            m_conditions = conditions;
            m_scoringMethods = scoringMethods;

            m_inflictedStatuses = new bool[statuses.Length];
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
                m_range = range,
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
