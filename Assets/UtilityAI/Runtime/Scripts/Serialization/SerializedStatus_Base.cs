using Stirge.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.UtilityAI
{
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

    public abstract class SerializedStatus_Base : ScriptableObject
    {
        [SerializeField, Range(0f, 5f)] protected float m_scoreScaling = 1f;
        [SerializeField] protected StatusStackType m_stackType;
        [SerializeField] protected StatusDurationType m_durationType;
        [SerializeField] protected string m_displayName;
        [SerializeField, Min(1)] protected int m_maxStacks;
        [SerializeField] protected SerializedCondition[] m_conditions;
        [SerializeField] protected SerializedScoringMethod_Base[] m_scoringMethods;

        public abstract Type statusType { get; }

        protected ICondition[] CreateRuntimeConditions()
        {
            int conditionCount = m_conditions.Length;
            ICondition[] conditions = new Condition[conditionCount];
            for (int i = 0; i < conditionCount; i++)
            {
                conditions[i] = m_conditions[i].CreateRuntimeCondition();
            }
            return conditions;
        }

        public abstract Status CreateRuntimeStatus();
    }
}
