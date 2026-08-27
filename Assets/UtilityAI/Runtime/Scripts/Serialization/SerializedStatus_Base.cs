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

    public abstract class SerializedStatus_Base : ScriptableObject, IScalable
    {
        [Header("Base Status Properties")]
        [SerializeField, Range(0f, 5f)] private float m_scoreScaling = 1f;
        [SerializeField] private StatusStackType m_stackType;
        [SerializeField] private StatusDurationType m_durationType;
        [SerializeField] private string m_displayName;
        [SerializeField, Min(1)] private int m_maxStacks;
        [SerializeField] private SerializedCondition[] m_conditions;

        public float ScoreScaling => m_scoreScaling;

        public void SetScoreScaling(float newScaling)
        {
            m_scoreScaling = newScaling;
        }

        public abstract Type statusType { get; }

        public abstract Status CreateRuntimeStatus();
    }
}
