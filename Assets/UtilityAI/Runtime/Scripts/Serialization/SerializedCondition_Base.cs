using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedCondition_Base : ScriptableObject
    {
        [SerializeField] private Operation m_operation;
        
        public abstract Type firstObjectType { get; }
        public abstract Type secondObjectType { get; }

        public abstract Condition CreateRuntimeCondition();
    }
}
