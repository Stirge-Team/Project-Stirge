using System;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    [CreateAssetMenu(menuName = "Stirge/Serialized Attack Data", fileName = "New Serialized Attack Data", order = 449)]
    public class SerializedAttackData : SerializedAttackData_Base
    {
        [SerializeField] private SerializedAttackNode_Base[] m_serializedAttackNodes;
        [SerializeField] private int[] m_nodeBindings;

        private AttackDataBuilder m_builder;

        public override AttackData CreateAttackData()
        {
            Deserialize();
            AttackData data = m_builder.Build();
            return data;
        }

        private void Deserialize()
        {
            if (m_builder != null)
            {
                return;
            }

            m_builder = new AttackDataBuilder();

            for (int i = 0, nodeCount = m_serializedAttackNodes.Length; i < nodeCount; i++)
            {
                SerializedAttackNode_Base serializedAttackNodeBase = m_serializedAttackNodes[i];
                serializedAttackNodeBase.AddAttackNode(m_builder);
                m_builder.AddBinding(m_nodeBindings[i]);
            }
        }

        private void OnValidate()
        {
            // release the builder
            m_builder = null;

            // get the number of editing nodes
            int serializedAttackNodeCount = m_serializedAttackNodes.Length;

            // Resize the bindings array to the size of the editing nodes array
            Array.Resize(ref m_nodeBindings, serializedAttackNodeCount);

            // Check if any Nodes have invalid bindings.
            // A Node is invalid if it has a null binding and is NOT the root Node (index 0, which is skipped).
            // A binding of -1 is reserved for the Root Node, which is not bound.
            // If a Node is invalid, remove it from the editing nodes array and resize the bindings array to match.
            for (int nodeIndex = 1, nodeCount = m_nodeBindings.Length; nodeIndex < nodeCount; ++nodeIndex)
            {
                int binding = m_nodeBindings[nodeIndex];

                // if the node that THIS node is bound to is invalid
                if (binding < 0 || binding >= serializedAttackNodeCount)
                {
                    RemoveElement(ref m_serializedAttackNodes, nodeIndex);
                    RemoveElement(ref m_nodeBindings, nodeIndex--);
                    nodeCount--;
                }
            }
        }

        private static void RemoveElement<T>(ref T[] array, int element)
        {
            for (int i = element, count = array.Length - 1; i < count; ++i)
            {
                array[i] = array[i + 1];
            }

            Array.Resize(ref array, array.Length - 1);
        }
    }
}
