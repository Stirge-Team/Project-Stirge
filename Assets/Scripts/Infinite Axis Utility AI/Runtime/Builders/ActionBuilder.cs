using System;
using UnityEngine;

namespace Stirge.UtilityAI.Builders
{
    using Core;
    using Action = Core.Action;

    public sealed class ActionBuilder<TAction> : IActionBuilder where TAction : Action, INotSetupable, new()
    {
        private readonly string m_name;
        
        public ActionBuilder(string name)
        {
            m_name = name;
        }

        public Type actionType => typeof(TAction);

        public Action Build()
        {
            Action action = Action.Create<TAction>();
            action.name = m_name;
            return action;
        }
    }

    public sealed class ActionBuilder<TAction, TArg> : IActionBuilder where TAction : Action, ISetupable<TArg>, new()
    {
        private readonly TArg m_arg;

        private readonly string m_name;

        public ActionBuilder(TArg arg, string name)
        {
            m_arg = arg;
            m_name = name;
        }

        public Type actionType => typeof(TAction);

        public Action Build()
        {
            TAction action = Action.Create<TAction, TArg>(m_arg);
            action.name = m_name;
            return action;
        }
    }

    public sealed class ActionBuilder<TAction, TArg0, TArg1> : IActionBuilder where TAction : Action, ISetupable<TArg0, TArg1>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;

        private readonly string m_name;

        public ActionBuilder(TArg0 arg0, TArg1 arg1, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_name = name;
        }

        public Type actionType => typeof(TAction);

        public Action Build()
        {
            TAction action = Action.Create<TAction, TArg0, TArg1>(m_arg0, m_arg1);
            action.name = m_name;
            return action;
        }
    }

    public sealed class ActionBuilder<TAction, TArg0, TArg1, TArg2> : IActionBuilder where TAction : Action, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;
        private readonly TArg2 m_arg2;

        private readonly string m_name;

        public ActionBuilder(TArg0 arg0, TArg1 arg1, TArg2 arg2, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_arg2 = arg2;
            m_name = name;
        }

        public Type actionType => typeof(TAction);

        public Action Build()
        {
            TAction action = Action.Create<TAction, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2);
            action.name = m_name;
            return action;
        }
    }

    public sealed class ActionBuilder : IActionBuilder
    {
        [SerializeField] private string m_name;
        [SerializeField] private Type m_actionType;
        [SerializeField] private object[] m_parameters;

        public ActionBuilder(Type actionType, object[] parameters = null, string name = "")
        {
            m_actionType = actionType;
            m_parameters = parameters;
            m_name = name;
        }

        public Type actionType => m_actionType;

        public Action Build()
        {
            Action action = m_parameters != null
                ? Action.Create(m_actionType, m_parameters)
                : Action.Create(m_actionType);
            action.name = m_name;
            return action;
        }
    }
}
