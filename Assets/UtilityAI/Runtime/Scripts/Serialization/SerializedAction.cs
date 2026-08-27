using Stirge.Combat;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Serialized Action", fileName = "New Serialized Action", order = 449)]
    public class SerializedAction : ScriptableObject, IScalable
    {
        [SerializeField, Range(0, 5f)] private float m_scoreScaling = 1f;
        [SerializeField] private string m_displayName;
        [SerializeField] private ActionType m_actionType;
        [SerializeField] private TimelineAsset m_timeline;
        [SerializeField, Min(0)] private float m_damage = 1f;
        [SerializeField, Min(0)] private float m_range = 1f;
        [SerializeField] private SerializedStatus_Base[] m_statuses = new SerializedStatus_Base[0];
        [SerializeField] private SerializedCondition[] m_conditions = new SerializedCondition[0];

        public float ScoreScaling => m_scoreScaling;

        public void SetScoreScaling(float newScaling)
        {
            m_scoreScaling = newScaling;
        }

        public Action CreateRuntimeAction()
        {
            int statusCount = m_statuses.Length;
            Status[] statuses = new Status[statusCount];
            for (int i = 0; i < statusCount; i++)
            {
                statuses[i] = m_statuses[i].CreateRuntimeStatus();
            }

            int conditionCount = m_conditions.Length;
            ICondition[] conditions = new ICondition[conditionCount];
            for (int i = 0; i < conditionCount; i++)
            {
                conditions[i] = m_conditions[i].CreateRuntimeCondition();
            }

            return Action.Create(m_scoreScaling, m_displayName, m_actionType, m_timeline, m_damage, m_range, statuses, conditions);
        }
    }
}
