using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;

    public class SerializedCondition<TCondition> : SerializedCondition_Base where TCondition : ICondition, INotSetupable, new()
    {
        public override Type ConditionType => typeof(TCondition);

        public sealed override ICondition CreateRuntimeCondition<TScorable>(TScorable scorable)
        {
            return Condition<TScorable>.Create<TCondition>(scorable);
        }
    }
    public class SerializedCondition<TCondition, TArg> : SerializedCondition_Base where TCondition : ICondition, ISetupable<TArg>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public override Type ConditionType => typeof(TCondition);

        public sealed override ICondition CreateRuntimeCondition<TScorable>(TScorable scorable)
        {
            return Condition<TScorable>.Create<TCondition, TArg>(m_arg, scorable);
        }
    }
    public class SerializedCondition<TCondition, TArg0, TArg1> : SerializedCondition_Base where TCondition : ICondition, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public override Type ConditionType => typeof(TCondition);

        public sealed override ICondition CreateRuntimeCondition<TScorable>(TScorable scorable)
        {
            return Condition<TScorable>.Create<TCondition, TArg0, TArg1>(m_arg0, m_arg1, scorable);
        }
    }
    public class SerializedCondition<TCondition, TArg0, TArg1, TArg2> : SerializedCondition_Base where TCondition : ICondition, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public override Type ConditionType => typeof(TCondition);

        public sealed override ICondition CreateRuntimeCondition<TScorable>(TScorable scorable)
        {
            return Condition<TScorable>.Create<TCondition, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2, scorable);
        }
    }
    public class SerializedCondition<TCondition, TArg0, TArg1, TArg2, TArg3> : SerializedCondition_Base where TCondition : ICondition, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;

        public override Type ConditionType => typeof(TCondition);

        public sealed override ICondition CreateRuntimeCondition<TScorable>(TScorable scorable)
        {
            return Condition<TScorable>.Create<TCondition, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3, scorable);
        }
    }
    public class SerializedCondition<TCondition, TArg0, TArg1, TArg2, TArg3, TArg4> : SerializedCondition_Base where TCondition : ICondition, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;
        [SerializeField, NameOverriden(4)] private TArg4 m_arg4;

        public override Type ConditionType => typeof(TCondition);

        public sealed override ICondition CreateRuntimeCondition<TScorable>(TScorable scorable)
        {
            return Condition<TScorable>.Create<TCondition, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4, scorable);
        }
    }
}
