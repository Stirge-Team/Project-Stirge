using Stirge.Serialization;
using Stirge.UtilityAI.Builders;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    public class AttackDataBuilder
    {
        [SerializeField] private readonly List<IAttackNodeBuilder> m_attackNodeBuilders = new();
        [SerializeField] private readonly List<int> m_attackNodeBindings = new();

        public AttackData Build()
        {
            AttackNode[] nodes = MakeAttackNodes();
            int[] nodeBindings = MakeAttackNodeBindings();

            AttackData data = AttackData.Create(nodes, nodeBindings);

            return data;
        }

        #region AttackNode Adders
        public void AddAttackNode<TAttackNode>(string name = "") where TAttackNode : AttackNode, INotSetupable, new()
        {
            m_attackNodeBuilders.Add(new AttackNodeBuilder<TAttackNode>(name));
            m_attackNodeBindings.Add(-1);
        }
        public void AddAttackNode<TAttackNode, TArg>(TArg arg, string name = "") where TAttackNode : AttackNode, ISetupable<TArg>, new()
        {
            m_attackNodeBuilders.Add(new AttackNodeBuilder<TAttackNode, TArg>(arg, name));
            m_attackNodeBindings.Add(-1);
        }
        public void AddAttackNode<TAttackNode, TArg0, TArg1>(TArg0 arg0, TArg1 arg1, string name = "") where TAttackNode : AttackNode, ISetupable<TArg0, TArg1>, new()
        {
            m_attackNodeBuilders.Add(new AttackNodeBuilder<TAttackNode, TArg0, TArg1>(arg0, arg1, name));
            m_attackNodeBindings.Add(-1);
        }
        public void AddAttackNode<TAttackNode, TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2, string name = "") where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2>, new()
        {
            m_attackNodeBuilders.Add(new AttackNodeBuilder<TAttackNode, TArg0, TArg1, TArg2>(arg0, arg1, arg2, name));
            m_attackNodeBindings.Add(-1);
        }
        public void AddAttackNode<TAttackNode, TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, string name = "") where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3>, new()
        {
            m_attackNodeBuilders.Add(new AttackNodeBuilder<TAttackNode, TArg0, TArg1, TArg2, TArg3>(arg0, arg1, arg2, arg3, name));
            m_attackNodeBindings.Add(-1);
        }
        public void AddAttackNode<TAttackNode, TArg0, TArg1, TArg2, TArg3, TArg4>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, string name = "") where TAttackNode : AttackNode, ISetupable<TArg0, TArg1, TArg2, TArg3, TArg4>, new()
        {
            m_attackNodeBuilders.Add(new AttackNodeBuilder<TAttackNode, TArg0, TArg1, TArg2, TArg3, TArg4>(arg0, arg1, arg2, arg3, arg4, name));
            m_attackNodeBindings.Add(-1);
        }
        public void AddAttackNode(Type attackNodeType, string name = "", params object[] parameters)
        {
            if (parameters is { Length: > 5 })
            {
                throw new ArgumentException($"Failed to add an attackNode of type {attackNodeType}. Too many {nameof(parameters)} were passed. The method supports up to 3.");
            }

            m_attackNodeBuilders.Add(new AttackNodeBuilder(attackNodeType, parameters, name));
            m_attackNodeBindings.Add(-1);
        }
        #endregion

        #region Makers
        private AttackNode[] MakeAttackNodes()
        {
            int count = m_attackNodeBuilders.Count;
            AttackNode[] nodes = new AttackNode[count];

            for (int i = 0; i < count; i++)
            {
                nodes[i] = m_attackNodeBuilders[i].Build();
            }

            return nodes;
        }
        private int[] MakeAttackNodeBindings()
        {
            int count = m_attackNodeBindings.Count;
            int[] bindings = new int[count];

            for (int i = 0; i < count; i++)
            {
                bindings[i] = m_attackNodeBindings[i];
            }

            return bindings;
        }
        #endregion
    }
}
