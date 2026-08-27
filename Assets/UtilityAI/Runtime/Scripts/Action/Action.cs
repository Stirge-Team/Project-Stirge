using Stirge.Combat;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.UtilityAI
{
    public class Action
    {
        // fields
        private float m_scoreScaling = 1f;
        private string m_displayName;
        private ActionType m_actionType;
        private TimelineAsset m_timeline;
        private float m_damage = 1f;
        private float m_range = 1f;
        private Status[] m_statuses;
        private ICondition[] m_conditions;
        private ScoringMethod[] m_scoringMethods;

        // properties
        public string displayName => m_displayName;
        public ActionType actionType => m_actionType;
        public TimelineAsset timeline => m_timeline;
        public float damage => m_damage;
        public float range => m_range;
        public Status[] statuses => m_statuses;
        public ICondition[] conditions => m_conditions;
        public ScoringMethod[] scoringMethods => m_scoringMethods;

        public float Evaluate(CombatEntity user, CombatEntity target)
        {
            if (!Enumerable.All(m_conditions, condition => condition.Evaluate()))
                return 0f;

            float baseScore = 0f;
            int methodCount = m_scoringMethods.Length;
            if (methodCount > 0)
            {
                for (int i = 0; i < methodCount; i++)
                {
                    baseScore += m_scoringMethods[i].Evaluate(user, target);
                }
                baseScore /= methodCount;
            }

            float statusScore = 0f;
            int statusCount = m_statuses.Length;
            if (statusCount > 0)
            {
                for (int i = 0; i < statusCount; i++)
                {
                    statusScore += m_statuses[i].Evaluate(user, target);
                }
                statusScore /= statusCount;
            }

            return (baseScore + statusScore) * m_scoreScaling;
        }

        public void Perform(CombatEntity user, CombatEntity target)
        {
            user.UseAction(m_timeline);
        }

        public static Action Create(float scaling, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range, Status[] statuses, ICondition[] conditions, ScoringMethod[] scoringMethods)
        {
            Action action = new()
            {
                m_scoreScaling = scaling,
                m_displayName = displayName,
                m_actionType = actionType,
                m_timeline = timeline,
                m_damage = damage,
                m_range = range,
                m_statuses = statuses,
                m_conditions = conditions,
                m_scoringMethods = scoringMethods
            };

            return action;
        }
    }
}
