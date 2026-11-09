using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedCondition_Base : ScriptableObject
    {
        public abstract Type ConditionType { get; }

        public abstract ICondition CreateRuntimeCondition<TScorable>(TScorable scorable) where TScorable : class, IScorable;
    }
}
