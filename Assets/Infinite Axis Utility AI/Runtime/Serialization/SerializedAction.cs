using System;
using UnityEngine;

namespace Stirge.UtilityAI.Serialization
{
    using Stirge.Serialization;
    using Builders;
    using Core;

    public abstract class SerializedAction<TAction> : SerializedAction_Base where TAction : Action, INotSetupable, new()
    {
        public sealed override Type actionType => typeof(TAction);

        public sealed override void AddAction(ActorBuilder builder)
        {
            builder.AddAction<TAction>(name);
        }
    }

    public abstract class SerializedAction<TAction, TArg> : SerializedAction_Base
        where TAction : Action, ISetupable<TArg>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg m_Arg;

        public sealed override Type actionType => typeof(TAction);

        public sealed override void AddAction(ActorBuilder builder)
        {
            builder.AddAction<TAction, TArg>(m_Arg, name);
        }
    }

    public abstract class SerializedAction<TAction, TArg0, TArg1> : SerializedAction_Base
        where TAction : Action, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_Arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_Arg1;

        public sealed override Type actionType => typeof(TAction);

        public sealed override void AddAction(ActorBuilder builder)
        {
            builder.AddAction<TAction, TArg0, TArg1>(m_Arg0, m_Arg1, name);
        }
    }

    public abstract class SerializedAction<TAction, TArg0, TArg1, TArg2> : SerializedAction_Base
        where TAction : Action, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_Arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_Arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_Arg2;

        public sealed override Type actionType => typeof(TAction);

        public sealed override void AddAction(ActorBuilder builder)
        {
            builder.AddAction<TAction, TArg0, TArg1, TArg2>(m_Arg0, m_Arg1, m_Arg2, name);
        }
    }
}
