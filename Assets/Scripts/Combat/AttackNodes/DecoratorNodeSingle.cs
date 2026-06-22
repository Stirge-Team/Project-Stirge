using Stirge.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public abstract class DecoratorNodeSingle : AttackNode, ISetupable<AttackNode>
    {
        [SerializeField] protected AttackNode m_node;

        public void Setup(AttackNode node)
        {
            m_node = node;
        }
    }
}
