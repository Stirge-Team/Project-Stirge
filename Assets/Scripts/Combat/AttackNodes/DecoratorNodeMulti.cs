using Stirge.Serialization;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public abstract class DecoratorNodeMulti : AttackNode, ISetupable<AttackNode[]>
    {
        [SerializeField] protected AttackNode[] m_nodes;

        public AttackNode[] Nodes => m_nodes;

        public void Setup(AttackNode[] nodes)
        {
            m_nodes = nodes;
        }
    }
}
