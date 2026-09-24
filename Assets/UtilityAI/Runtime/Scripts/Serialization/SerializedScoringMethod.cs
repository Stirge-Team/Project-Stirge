using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Serialization;

    public abstract class SerializedScoringMethod<TScoringMethod> : SerializedScoringMethod_Base where TScoringMethod : ScoringMethod, INotSetupable, new()
    {
        public override Type scoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod CreateRuntimeScoringMethod()
        {
            return ScoringMethod.Create<TScoringMethod>(m_scoreScaling);
        }
    }
    public abstract class SerializedScoringMethod<TScoringMethod, TArg> : SerializedScoringMethod_Base where TScoringMethod : ScoringMethod, ISetupable<TArg>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public override Type scoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod CreateRuntimeScoringMethod()
        {
            return ScoringMethod.Create<TScoringMethod, TArg>(m_arg, m_scoreScaling);
        }
    }
    public abstract class SerializedScoringMethod<TScoringMethod, TArg0, TArg1> : SerializedScoringMethod_Base where TScoringMethod : ScoringMethod, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public override Type scoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod CreateRuntimeScoringMethod()
        {
            return ScoringMethod.Create<TScoringMethod, TArg0, TArg1>(m_arg0, m_arg1, m_scoreScaling);
        }
    }

    public abstract class SerializedScoringMethod<TScoringMethod, TArg0, TArg1, TArg2> : SerializedScoringMethod_Base where TScoringMethod : ScoringMethod, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public override Type scoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod CreateRuntimeScoringMethod()
        {
            return ScoringMethod.Create<TScoringMethod, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2, m_scoreScaling);
        }
    }

    public abstract class SerializedScoringMethod<TScoringMethod, TArg0, TArg1, TArg2, TArg3> : SerializedScoringMethod_Base where TScoringMethod : ScoringMethod, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;

        public sealed override Type scoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod CreateRuntimeScoringMethod()
        {
            return ScoringMethod.Create<TScoringMethod, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3, m_scoreScaling);
        }
    }

    public abstract class SerializedScoringMethod<TScoringMethod, TArg0, TArg1, TArg2, TArg3, TArg4> : SerializedScoringMethod_Base where TScoringMethod : ScoringMethod, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;
        [SerializeField, NameOverriden(4)] private TArg4 m_arg4;

        public sealed override Type scoringMethodType => typeof(TScoringMethod);

        public sealed override ScoringMethod CreateRuntimeScoringMethod()
        {
            return ScoringMethod.Create<TScoringMethod, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4, m_scoreScaling);
        }
    }
}
