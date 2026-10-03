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
        [SerializeField] protected StatusInflictCondition m_inflictCondition;
        [SerializeField] protected float m_inflictDelay;
        [SerializeField] protected SerializedCondition_Base[] m_conditions = new SerializedCondition_Base[0];
        [SerializeField] protected SerializedScoringMethod_Base[] m_scoringMethods = new SerializedScoringMethod_Base[0];

        public abstract Type StatusType { get; }

        protected ICondition[] CreateRuntimeConditions(Status status)
        {
            int conditionCount = m_conditions.Length;
            ICondition[] conditions = new ICondition[conditionCount];
            for (int i = 0; i < conditionCount; i++)
            {
                conditions[i] = m_conditions[i].CreateRuntimeCondition(status);
            }
            return conditions;
        }
        protected IScoringMethod[] CreateRuntimeScoringMethods(Status status)
        {
            int scoringMethodCount = m_scoringMethods.Length;
            IScoringMethod[] scoringMethods = new IScoringMethod[scoringMethodCount];
            for (int i = 0; i < scoringMethodCount; i++)
            {
                scoringMethods[i] = m_scoringMethods[i].CreateRuntimeScoringMethod(status);
            }
            return scoringMethods;
        }

        public abstract Status CreateRuntimeStatus();
    }
}
