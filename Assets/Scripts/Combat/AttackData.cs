using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class AttackData
    {
        [SerializeReference] private AttackNode m_root;

        public AttackNode[] EvaluateSequence()
        {
            List<AttackNode> sequence = new();
            m_root.Evaluate(sequence);
            return sequence.ToArray();
        }

        public static AttackData Create(AttackNode[] attackNodes, int[] bindings)
        {
            AttackData data = new AttackData();
            int attackNodesCount = attackNodes.Length;
            if (attackNodesCount > 0)
            {
                data.m_root = attackNodes[0];

                // Set up all the Bindings.
                // Ignore the Root node as it cannot be bound to any other Node AND has an invalid binding of -1
                for (int i = 1; i < attackNodesCount; i++)
                {
                    AttackNode currentNode = attackNodes[i];
                    int currentBinding = bindings[i];

                    // Check if the Node that THIS Node is bound to is a valid Decorator
                    if (attackNodes[currentBinding] is IDecoratorNode boundNode)
                    {
                        boundNode.AddAttackNode(currentNode);
                    }
                    else
                    {
                        Debug.LogError($"SerializedAttackData is invalid, an AttackNode is bound to a non-DecoratorNode. Check your bindings!");
                    }
                }
            }
            return data;
        }
    }
}
