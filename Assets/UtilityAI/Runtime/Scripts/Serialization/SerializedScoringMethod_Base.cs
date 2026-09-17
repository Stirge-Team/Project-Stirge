using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedScoringMethod_Base : ScriptableObject
    {
        [SerializeField, Range(0f, 5f)] protected float m_scoreScaling = 1f;

        public abstract Type scoringMethodType { get; }

        public abstract ScoringMethod CreateRuntimeScoringMethod();
    }
}
