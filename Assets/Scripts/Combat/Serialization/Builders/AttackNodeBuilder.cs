using Stirge.Serialization;
using Stirge.UtilityAI.Builders;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    public sealed class AttackNodeBuilder<TAttackNode> : IAttackNodeBuilder where TAttackNode : AttackNode, INotSetupable, new()
    {
        private readonly string m_name;

        public AttackNodeBuilder(string name)
        {
            m_name = name;
        }

        public Type attackNodeType => typeof(TAttackNode);

        public AttackNode Build()
        {
            AttackNode node = AttackNode.Create<TAttackNode>();
            node.name = m_name;
            return node;
        }

        public bool AreEqual(object[] parameters)
        {
            return parameters == null || parameters.Length == 0;
        }
    }

    public sealed class AttackNodeBuilder<TAttackNode, TArg> : IAttackNodeBuilder where TAttackNode : AttackNode, ISetupable<TArg>, new()
    {
        private readonly TArg m_arg;

        private readonly string m_name;

        public AttackNodeBuilder(TArg arg, string name)
        {
            m_arg = arg;
            
            m_name = name;
        }

        public Type attackNodeType => typeof(TAttackNode);

        public AttackNode Build()
        {
            AttackNode node = AttackNode.Create<TAttackNode, TArg>(m_arg);
            node.name = m_name;
            return node;
        }

        public bool AreEqual(TArg arg)
        {
            return EqualityComparer<TArg>.Default.Equals(m_arg, arg);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg == null ? parameters[0] == null : parameters[0] is TArg parameter && EqualityComparer<TArg>.Default.Equals(m_arg, parameter);
        }
    }

    public sealed class AttackNodeBuilder<TAttackNode, TArg0, TArg1> : IAttackNodeBuilder where TAttackNode : AttackNode, ISetupable<TArg0, TArg1>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;

        private readonly string m_name;

        public AttackNodeBuilder(TArg0 arg0, TArg1 arg1, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            
            m_name = name;
        }

        public Type attackNodeType => typeof(TAttackNode);

        public AttackNode Build()
        {
            AttackNode node = AttackNode.Create<TAttackNode, TArg0, TArg1>(m_arg0, m_arg1);
            node.name = m_name;
            return node;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                   EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1);
        }
    }

    public sealed class AttackNodeBuilder<TAttackNode, TArg0, TArg1, TArg2> : IAttackNodeBuilder where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;
        private readonly TArg2 m_arg2;

        private readonly string m_name;

        public AttackNodeBuilder(TArg0 arg0, TArg1 arg1, TArg2 arg2, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_arg2 = arg2;

            m_name = name;
        }

        public Type attackNodeType => typeof(TAttackNode);

        public AttackNode Build()
        {
            TAttackNode node = AttackNode.Create<TAttackNode, TArg0, TArg1, TArg2>(m_arg0, m_arg1, m_arg2);
            node.name = m_name;
            return node;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1) &&
                EqualityComparer<TArg2>.Default.Equals(m_arg2, arg2);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1) &&
                   m_arg2 == null ? parameters[2] == null : parameters[2] is TArg2 parameter2 && EqualityComparer<TArg2>.Default.Equals(m_arg2, parameter2);
        }
    }

    public sealed class AttackNodeBuilder<TAttackNode, TArg0, TArg1, TArg2, TArg3> : IAttackNodeBuilder where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;
        private readonly TArg2 m_arg2;
        private readonly TArg3 m_arg3;

        private readonly string m_name;

        public AttackNodeBuilder(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_arg2 = arg2;
            m_arg3 = arg3;

            m_name = name;
        }

        public Type attackNodeType => typeof(TAttackNode);

        public AttackNode Build()
        {
            TAttackNode node = AttackNode.Create<TAttackNode, TArg0, TArg1, TArg2, TArg3>(m_arg0, m_arg1, m_arg2, m_arg3);
            node.name = m_name;
            return node;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1) &&
                EqualityComparer<TArg2>.Default.Equals(m_arg2, arg2) &&
                EqualityComparer<TArg3>.Default.Equals(m_arg3, arg3);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1) &&
                   m_arg2 == null ? parameters[2] == null : parameters[2] is TArg2 parameter2 && EqualityComparer<TArg2>.Default.Equals(m_arg2, parameter2) &&
                   m_arg3 == null ? parameters[3] == null : parameters[3] is TArg3 parameter3 && EqualityComparer<TArg3>.Default.Equals(m_arg3, parameter3);
        }
    }

    public sealed class AttackNodeBuilder<TAttackNode, TArg0, TArg1, TArg2, TArg3, TArg4> : IAttackNodeBuilder where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
    {
        private readonly TArg0 m_arg0;
        private readonly TArg1 m_arg1;
        private readonly TArg2 m_arg2;
        private readonly TArg3 m_arg3;
        private readonly TArg4 m_arg4;

        private readonly string m_name;

        public AttackNodeBuilder(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, string name)
        {
            m_arg0 = arg0;
            m_arg1 = arg1;
            m_arg2 = arg2;
            m_arg3 = arg3;
            m_arg4 = arg4;

            m_name = name;
        }

        public Type attackNodeType => typeof(TAttackNode);

        public AttackNode Build()
        {
            TAttackNode node = AttackNode.Create<TAttackNode, TArg0, TArg1, TArg2, TArg3, TArg4>(m_arg0, m_arg1, m_arg2, m_arg3, m_arg4);
            node.name = m_name;
            return node;
        }

        public bool AreEqual(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return EqualityComparer<TArg0>.Default.Equals(m_arg0, arg0) &&
                EqualityComparer<TArg1>.Default.Equals(m_arg1, arg1) &&
                EqualityComparer<TArg2>.Default.Equals(m_arg2, arg2) &&
                EqualityComparer<TArg3>.Default.Equals(m_arg3, arg3) &&
                EqualityComparer<TArg4>.Default.Equals(m_arg4, arg4);
        }

        public bool AreEqual(object[] parameters)
        {
            if (parameters == null)
            {
                return false;
            }

            return m_arg0 == null ? parameters[0] == null : parameters[0] is TArg0 parameter0 && EqualityComparer<TArg0>.Default.Equals(m_arg0, parameter0) &&
                   m_arg1 == null ? parameters[1] == null : parameters[1] is TArg1 parameter1 && EqualityComparer<TArg1>.Default.Equals(m_arg1, parameter1) &&
                   m_arg2 == null ? parameters[2] == null : parameters[2] is TArg2 parameter2 && EqualityComparer<TArg2>.Default.Equals(m_arg2, parameter2) &&
                   m_arg3 == null ? parameters[3] == null : parameters[3] is TArg3 parameter3 && EqualityComparer<TArg3>.Default.Equals(m_arg3, parameter3) &&
                   m_arg4 == null ? parameters[4] == null : parameters[4] is TArg4 parameter4 && EqualityComparer<TArg4>.Default.Equals(m_arg4, parameter4);
        }
    }

    public sealed class AttackNodeBuilder : IAttackNodeBuilder
    {
        [SerializeField] private string m_name;
        [SerializeField] private Type m_attackNodeType;
        [SerializeField] private object[] m_parameters;
        public AttackNodeBuilder(Type attackNodeType, object[] parameters = null, string name = "")
        {
            m_attackNodeType = attackNodeType;
            m_parameters = parameters;
            m_name = name;
        }

        public Type attackNodeType => m_attackNodeType;

        public bool AreEqual(object[] parameters)
        {
            if (m_parameters == null)
            {
                return parameters == null;
            }

            if (parameters == null)
            {
                return false;
            }

            if (m_parameters.Length != parameters.Length)
            {
                return false;
            }

            for (int i = 0, count = m_parameters.Length; i < count; ++i)
            {
                if (m_parameters[i] == null)
                {
                    if (parameters[i] != null)
                    {
                        return false;
                    }
                }
                else if (!m_parameters[i].Equals(parameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public AttackNode Build()
        {
            AttackNode node = m_parameters != null
                ? AttackNode.Create(m_attackNodeType, m_parameters)
                : AttackNode.Create(m_attackNodeType);
            node.name = m_name;
            return node;
        }
    }
}
