using Stirge.Combat;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.UtilityAI
{
    [CreateAssetMenu(menuName = "Stirge/Action", fileName = "New Action", order = 450)]
    public class Action : ScriptableObject
    {
        [SerializeField, Range(0, 5f)] private float m_scaling = 1f;
        [SerializeField] private string m_displayName;
        [SerializeField] private ActionType m_actionType;
        [SerializeField] private TimelineAsset m_timeline;
        [SerializeField, Min(0)] private float m_damage = 1f;
        [SerializeField, Min(0)] private float m_range = 1f;
        [SerializeField] private SerializedStatus_Base[] m_statuses = new SerializedStatus_Base[0];
        [SerializeField] private Condition[] m_conditions = new Condition[0];

        private Status[] m_runtimeStatuses;

        public void Initialise()
        {
            int count = m_statuses.Length;
            m_runtimeStatuses = new Status[count];
            for (int i = 0; i < count; i++)
            {
                m_runtimeStatuses[i] = m_statuses[i].CreateRuntimeStatus();
            }
        }

        public float Evaluate(CombatEntity user, CombatEntity target)
        {
            if (!Enumerable.All(m_conditions, condition => condition.Evaluate(user, target)))
                return 0f;
            float actionScore = m_damage / m_range;

            float statusScore = 0;
            int count = m_statuses.Length;
            for (int i = 0; i < count; i++)
            {
                statusScore += m_statuses[i].Evaluate(m_runtimeStatuses[i], user, target);
            }
            statusScore /= count;

            return (actionScore + statusScore) * m_scaling;
        }

        public void Perform(CombatEntity user, CombatEntity target)
        {
            user.UseAction(m_timeline);
        }
    }
}
