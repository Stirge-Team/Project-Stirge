using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;

    public abstract class SerializedScoringMethod<T, TScoringMethod> : SerializedScoringMethod_Base<T> where T : IScorable where TScoringMethod : ScoringMethod<T>, INotSetupable, new()
    {
        public override Type ScoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod<T> CreateRuntimeScoringMethod(T scorable)
        {
            return ScoringMethod<T>.Create<TScoringMethod>(scorable, m_scoreScaling);
        }
    }
    public abstract class SerializedScoringMethod<T, TScoringMethod, TArg> : SerializedScoringMethod_Base<T> where T : IScorable where TScoringMethod : ScoringMethod<T>, ISetupable<TArg>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public override Type ScoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod<T> CreateRuntimeScoringMethod(T scorable)
        {
            return ScoringMethod<T>.Create<TScoringMethod, TArg>(scorable, m_arg, m_scoreScaling);
        }
    }
    public abstract class SerializedScoringMethod<T, TScoringMethod, TArg0, TArg1> : SerializedScoringMethod_Base<T> where T : IScorable where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public override Type ScoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod<T> CreateRuntimeScoringMethod(T scorable)
        {
            return ScoringMethod<T>.Create<TScoringMethod, TArg0, TArg1>(scorable, m_arg0, m_arg1, m_scoreScaling);
        }
    }
    public abstract class SerializedScoringMethod<T, TScoringMethod, TArg0, TArg1, TArg2> : SerializedScoringMethod_Base<T> where T : IScorable where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public override Type ScoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod<T> CreateRuntimeScoringMethod(T scorable)
        {
            return ScoringMethod<T>.Create<TScoringMethod, TArg0, TArg1, TArg2>(scorable, m_arg0, m_arg1, m_arg2, m_scoreScaling);
        }
    }
    public abstract class SerializedScoringMethod<T, TScoringMethod, TArg0, TArg1, TArg2, TArg3> : SerializedScoringMethod_Base<T> where T : IScorable where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;

        public sealed override Type ScoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod<T> CreateRuntimeScoringMethod(T scorable)
        {
            return ScoringMethod<T>.Create<TScoringMethod, TArg0, TArg1, TArg2, TArg3>(scorable, m_arg0, m_arg1, m_arg2, m_arg3, m_scoreScaling);
        }
    }
    public abstract class SerializedScoringMethod<T, TScoringMethod, TArg0, TArg1, TArg2, TArg3, TArg4> : SerializedScoringMethod_Base<T> where T : IScorable where TScoringMethod : ScoringMethod<T>, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;
        [SerializeField, NameOverriden(3)] private TArg4 m_arg4;

        public sealed override Type ScoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod<T> CreateRuntimeScoringMethod(T scorable)
        {
            return ScoringMethod<T>.Create<TScoringMethod, TArg0, TArg1, TArg2, TArg3, TArg4>(scorable, m_arg0, m_arg1, m_arg2, m_arg3, m_arg4, m_scoreScaling);
        }
    }
}
