using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class SimultaneousAttackNode : AttackNode
    {
        public SimultaneousAttackNode()
        {
            m_nodes = new AttackNode[0];
        }
        
        [SerializeReference] private AttackNode[] m_nodes;

        public AttackNode[] Nodes => m_nodes;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            activeNodes.Add(this);
        }
    }
}
