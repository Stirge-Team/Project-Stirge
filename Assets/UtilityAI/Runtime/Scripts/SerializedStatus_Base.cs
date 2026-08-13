using Stirge.Combat;
using System;
using System.Linq;
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
        [Header("Base Properties")]
        [SerializeField, Range(0f, 5f)] private float m_scaling = 1f;
        [SerializeField] private StatusStackType m_stackType;
        [SerializeField] private StatusDurationType m_durationType;
        [SerializeField] private string m_displayName;
        [SerializeField, Min(1)] private int m_maxStacks;
        [SerializeField] private Condition[] m_conditions = new Condition[0];

        public abstract Type statusType { get; }

        public StatusStackType stackType => m_stackType;
        public StatusDurationType durationType => m_durationType;
        public string displayName => m_displayName;
        public int maxStacks => m_maxStacks;

        public abstract Status CreateRuntimeStatus();

        public float Evaluate(Status status, CombatEntity user, CombatEntity target)
        {
            if (!Enumerable.All(m_conditions, condition => condition.Evaluate(user, target)))
                return 0f;
            float score = status.Evaluate(user, target);
            return score * m_scaling;
        }
    }
}
