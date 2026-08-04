using System;
using UnityEngine;
using Stirge.Serialization;

namespace Stirge.Combat.Attacks.Serialization
{
    public abstract class SerializedAttackNode<TAttackNode> : SerializedAttackNode_Base where TAttackNode : AttackNode, INotSetupable, new()
    {
        public sealed override Type attackNodeType => typeof(TAttackNode);

        public sealed override void AddAttackNode(AttackDataBuilder builder)
        {
            builder.AddAttackNode<TAttackNode>(name);
        }
    }

    public abstract class SerializedAttackNode<TAttackNode, TArg> : SerializedAttackNode_Base where TAttackNode : AttackNode, ISetupable<TArg>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg m_arg;

        public sealed override Type attackNodeType => typeof(TAttackNode);

        public sealed override void AddAttackNode(AttackDataBuilder builder)
        {
            builder.AddAttackNode<TAttackNode, TArg>(m_arg, name);
        }
    }

    public abstract class SerializedAttackNode<TAttackNode, TArg0, TArg1> : SerializedAttackNode_Base where TAttackNode : AttackNode, ISetupable<TArg0, TArg1>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;

        public sealed override Type attackNodeType => typeof(TAttackNode);

        public sealed override void AddAttackNode(AttackDataBuilder builder)
        {
            builder.AddAttackNode<TAttackNode, TArg0, TArg1>(m_arg0, m_arg1, name);
        }
    }

    public abstract class SerializedAttackNode<TAttackNode, TArg0, TArg1, TArg2> : SerializedAttackNode_Base where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;

        public sealed override Type attackNodeType => typeof(TAttackNode);

        public sealed override void AddAttackNode(AttackDataBuilder builder)
        {
            builder.AddAttackNode<TAttackNode, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2, name);
        }
    }

    public abstract class SerializedAttackNode<TAttackNode, TArg0, TArg1, TArg2, TArg3> : SerializedAttackNode_Base where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;

        public sealed override Type attackNodeType => typeof(TAttackNode);

        public sealed override void AddAttackNode(AttackDataBuilder builder)
        {
            builder.AddAttackNode<TAttackNode, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3, name);
        }
    }

    public abstract class SerializedAttackNode<TAttackNode, TArg0, TArg1, TArg2, TArg3, TArg4> : SerializedAttackNode_Base where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        [SerializeField, NameOverriden(0)] private TArg0 m_arg0;
        [SerializeField, NameOverriden(1)] private TArg1 m_arg1;
        [SerializeField, NameOverriden(2)] private TArg2 m_arg2;
        [SerializeField, NameOverriden(3)] private TArg3 m_arg3;
        [SerializeField, NameOverriden(4)] private TArg4 m_arg4;

        public sealed override Type attackNodeType => typeof(TAttackNode);

        public sealed override void AddAttackNode(AttackDataBuilder builder)
        {
            builder.AddAttackNode<TAttackNode, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4, name);
        }
    }
}
