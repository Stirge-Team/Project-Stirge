using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    /// <summary>
    /// This setup is required for valid Contravariance.
    /// </summary>
    /// <typeparam name="TScorable"></typeparam>
    public abstract class SerializedScoringMethod_Base<T> : ScriptableObject where T : IScorable
    {
        [SerializeField, Range(0f, 5f)] protected float m_scoreScaling = 1f;

        public abstract Type ScoringMethodType { get; }

        public abstract ScoringMethod<T> CreateRuntimeScoringMethod(T scorable);
    }
}
