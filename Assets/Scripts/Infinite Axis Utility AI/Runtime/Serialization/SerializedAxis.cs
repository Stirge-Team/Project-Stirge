using UnityEngine;
using System;

namespace Stirge.UtilityAI.Serialization
{
    using Attributes;
    using Builders;
    using Core;
    using Stirge.Serialization;

    public abstract class SerializedAxis<TAxis> : SerializedAxis_Base where TAxis : Axis, INotSetupable, new()
    {
        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis>(name);
        }
    }

    public abstract class SerializedAxis<TAxis, TArg> : SerializedAxis_Base where TAxis : Axis, ISetupable<TArg>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis, TArg>(m_arg, name);
        }
    }

    public abstract class SerializedAxis<TAxis, TArg0, TArg1> : SerializedAxis_Base where TAxis : Axis, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis, TArg0, TArg1>(m_arg0, m_arg1, name);
        }
    }

    public abstract class SerializedAxis<TAxis, TArg0, TArg1, TArg2> : SerializedAxis_Base where TAxis : Axis, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2, name);
        }
    }
}
