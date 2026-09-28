using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedStatus_Base : ScriptableObject
    {
        [SerializeField, Range(0f, 5f)] protected float m_scoreScaling = 1f;
        [SerializeField] protected string m_displayName;
        [SerializeField] protected StatusStackType m_stackType;
        [SerializeField, Range(1, 30)] protected int m_maxStacks;
        [SerializeField] protected StatusDurationType m_durationType;
        [SerializeField] protected float m_duration;
        [SerializeField] protected SerializedCondition[] m_conditions;
        [SerializeField] protected SerializedScoringMethod_Base<Status>[] m_scoringMethods;

        public abstract Type StatusType { get; }

        protected ICondition[] CreateRuntimeConditions()
        {
            int conditionCount = m_conditions.Length;
            ICondition[] conditions = new ICondition[conditionCount];
            for (int i = 0; i < conditionCount; i++)
            {
                conditions[i] = m_conditions[i].CreateRuntimeCondition();
            }
            return conditions;
        }
        protected IScoringMethod<Status>[] CreateRuntimeScoringMethods()
        {
            int scoringMethodCount = m_scoringMethods.Length;
            IScoringMethod<Status>[] scoringMethods = new IScoringMethod<Status>[scoringMethodCount];
            for (int i = 0; i < scoringMethodCount; i++)
            {
                scoringMethods[i] = m_scoringMethods[i].CreateRuntimeScoringMethod();
            }
            return scoringMethods;
        }

        public abstract Status CreateRuntimeStatus();
    }
}
