using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class SequenceAttackNode : AttackNode
    {
        public SequenceAttackNode()
        {
            m_nodes = new AttackNode[0];
        }
        
        [SerializeReference] private AttackNode[] m_nodes;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            foreach (AttackNode node in m_nodes)
            {
                node.Evaluate(activeNodes);
            }
        }

        public override float EvaluateTime()
        {
            return 0f;
        }
    }
}
