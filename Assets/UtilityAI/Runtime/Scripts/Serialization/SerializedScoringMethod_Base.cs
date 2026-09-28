using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedScoringMethod_Base<TScorable> : ScriptableObject where TScorable : IScorable
    {
        [SerializeField, Range(0f, 5f)] protected float m_scoreScaling = 1f;

        public abstract Type scoringMethodType { get; }

        public abstract IScoringMethod<TScorable> CreateRuntimeScoringMethod();
    }
}
