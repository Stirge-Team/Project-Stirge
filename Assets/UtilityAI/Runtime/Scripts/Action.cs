using Stirge.Combat;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.UtilityAI
{
    public class Action
    {
        private float m_scaling = 1f;
        private string m_displayName;
        private ActionType m_actionType;
        private TimelineAsset m_timeline;
        private float m_damage = 1f;
        private float m_range = 1f;

        private Status[] m_statuses;
        private ICondition[] m_conditions;

        public float Evaluate(CombatEntity user, CombatEntity target)
        {
            if (!Enumerable.All(m_conditions, condition => condition.Evaluate()))
                return 0f;

            float actionScore = m_damage / m_range;
            float statusScore = 0;
            int count = m_statuses.Length;
            for (int i = 0; i < count; i++)
            {
                statusScore += m_statuses[i].Evaluate(user, target);
            }
            statusScore /= count;

            return (actionScore + statusScore) * m_scaling;
        }

        public void Perform(CombatEntity user, CombatEntity target)
        {
            user.UseAction(m_timeline);
        }

        public static Action Create(float scaling, string displayName, ActionType actionType, TimelineAsset timeline, float damage, float range, Status[] statuses, ICondition[] conditions)
        {
            Action action = new()
            {
                m_scaling = scaling,
                m_displayName = displayName,
                m_actionType = actionType,
                m_timeline = timeline,
                m_damage = damage,
                m_range = range,
                m_statuses = statuses,
                m_conditions = conditions
            };

            return action;
        }
    }
}
