using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class SelectAttackNode : AttackNode
    {
        public SelectAttackNode()
        {
            m_nodes = new AttackNode[0];
        }

        [SerializeReference] private AttackNode[] m_nodes = new AttackNode[0];

        private int m_chosenIndex = -1;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_chosenIndex = Random.Range(0, m_nodes.Length - 1);
            m_nodes[m_chosenIndex].Evaluate(activeNodes);
        }

        public override float EvaluateTime()
        {
            return 0;
        }
    }
}
