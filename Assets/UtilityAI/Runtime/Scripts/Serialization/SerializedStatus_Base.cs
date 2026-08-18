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
        [SerializeField] protected SerializedStatusData m_statusData;

        public abstract Type statusType { get; }

        public Status CreateRuntimeStatus()
        {
            Status status = _CreateRuntimeStatus();
            status.Init(m_statusData.CreateRuntimeStatusData());
            return status;
        }
        protected abstract Status _CreateRuntimeStatus();
    }
}
