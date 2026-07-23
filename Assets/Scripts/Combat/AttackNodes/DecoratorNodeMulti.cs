using Stirge.Serialization;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public abstract class DecoratorNodeMulti : AttackNode, IDecoratorNode, ISetupable<AttackNode[]>
    {
        [SerializeField] protected AttackNode[] m_nodes;

        public AttackNode[] Nodes => m_nodes;

        public void Setup(AttackNode[] nodes)
        {
            m_nodes = nodes;
        }

        void IDecoratorNode.AddAttackNode(AttackNode attackNode)
        {
            m_nodes ??= new AttackNode[0];
            
            int arraySize = m_nodes.Length;
            System.Array.Resize(ref m_nodes, arraySize + 1);
            m_nodes[arraySize] = attackNode;
        }
    }
}
