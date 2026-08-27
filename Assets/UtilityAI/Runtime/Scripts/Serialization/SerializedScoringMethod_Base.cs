using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedScoringMethod_Base : ScriptableObject
    {
        [SerializeField] protected float m_scoreScaling;

        public abstract Type scoringMethodType { get; }

        public abstract ScoringMethod CreateRuntimeScoringMethod();
    }
}
