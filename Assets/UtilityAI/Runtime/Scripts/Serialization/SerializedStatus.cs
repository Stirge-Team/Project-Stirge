using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;

    public abstract class SerializedStatus<TStatus> : SerializedStatus_Base where TStatus : Status, INotSetupable, new()
    {
        public override Type statusType => typeof(TStatus);
        
        public sealed override Status CreateRuntimeStatus()
        {
            return Status.Create<TStatus>(m_scoreScaling, m_stackType, m_durationType, m_displayName, m_maxStacks, CreateRuntimeConditions());
        }
    }
    public abstract class SerializedStatus<TStatus, TArg> : SerializedStatus_Base where TStatus : Status, ISetupable<TArg>, new()
    {
        [Header("Status Properties")]
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public override Type statusType => typeof(TStatus);

        public sealed override Status CreateRuntimeStatus()
        {
            return Status.Create<TStatus, TArg>(m_arg, m_scoreScaling, m_stackType, m_durationType, m_displayName, m_maxStacks, CreateRuntimeConditions());
        }
    }
    public abstract class SerializedStatus<TStatus, TArg0, TArg1> : SerializedStatus_Base where TStatus : Status, ISetupable<TArg0, TArg1>, new()
    {
        [Header("Status Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public override Type statusType => typeof(TStatus);

        public sealed override Status CreateRuntimeStatus()
        {
            return Status.Create<TStatus, TArg0, TArg1>(m_arg0, m_arg1, m_scoreScaling, m_stackType, m_durationType, m_displayName, m_maxStacks, CreateRuntimeConditions());
        }
    }

    public abstract class SerializedStatus<TStatus, TArg0, TArg1, TArg2> : SerializedStatus_Base where TStatus : Status, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [Header("Status Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public override Type statusType => typeof(TStatus);

        public sealed override Status CreateRuntimeStatus()
        {
            return Status.Create<TStatus, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2, m_scoreScaling, m_stackType, m_durationType, m_displayName, m_maxStacks, CreateRuntimeConditions());
        }
    }

    public abstract class SerializedStatus<TStatus, TArg0, TArg1, TArg2, TArg3> : SerializedStatus_Base where TStatus : Status, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        [Header("Status Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;

        public sealed override Type statusType => typeof(TStatus);

        public sealed override Status CreateRuntimeStatus()
        {
            return Status.Create<TStatus, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3, m_scoreScaling, m_stackType, m_durationType, m_displayName, m_maxStacks, CreateRuntimeConditions());
        }
    }

    public abstract class SerializedStatus<TStatus, TArg0, TArg1, TArg2, TArg3, TArg4> : SerializedStatus_Base where TStatus : Status, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        [Header("Status Properties")]
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;
        [SerializeField, NameOverriden(4)] private TArg4 m_arg4;

        public sealed override Type statusType => typeof(TStatus);

        public sealed override Status CreateRuntimeStatus()
        {
            return Status.Create<TStatus, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4, m_scoreScaling, m_stackType, m_durationType, m_displayName, m_maxStacks, CreateRuntimeConditions());
        }
    }
}
