using UnityEngine;
using System;

namespace Stirge.UtilityAI.Serialization
{
    using Attributes;
    using Builders;
    using Core;

    public abstract class SerializedAxis<TAxis> : SerializedAxis_Base where TAxis : Axis, INotSetupable, new()
    {
        [SerializeField] private SerializableCallback<float> m_getValue;
        
        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis>(m_getValue, name);
        }
    }

    public abstract class SerializedAxis<TAxis, TArg> : SerializedAxis_Base
        where TAxis : Axis, ISetupable<TArg>, new()
    {
        [SerializeField] private SerializableCallback<float> m_getValue;
        [SerializeField, NameOverriden(0)] private TArg m_Arg;

        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis, TArg>(m_getValue, m_Arg, name);
        }
    }

    public abstract class SerializedAxis<TAxis, TArg0, TArg1> : SerializedAxis_Base
        where TAxis : Axis, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField] private SerializableCallback<float> m_getValue;
        [SerializeField, NameOverriden(0)] private TArg0 m_Arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_Arg1;

        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis, TArg0, TArg1>(m_getValue, m_Arg0, m_Arg1, name);
        }
    }

    public abstract class SerializedAxis<TAxis, TArg0, TArg1, TArg2> : SerializedAxis_Base
        where TAxis : Axis, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField] private SerializableCallback<float> m_getValue;
        [SerializeField, NameOverriden(0)] private TArg0 m_Arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_Arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_Arg2;

        public sealed override Type axisType => typeof(TAxis);

        public sealed override void AddAxis(ActorBuilder builder)
        {
            builder.AddAxis<TAxis, TArg0, TArg1, TArg2>(m_getValue, m_Arg0, m_Arg1, m_Arg2, name);
        }
    }
}
