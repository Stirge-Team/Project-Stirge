using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class SelectAttackNode : AttackNode
    {
        [SerializeField] private AttackNode[] m_nodes;

        private int m_chosenIndex = -1;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_chosenIndex = Random.Range(0, m_nodes.Length - 1);
            m_nodes[m_chosenIndex].Evaluate(activeNodes);
        }

        public override float EvaluateTime()
        {
            return m_nodes[m_chosenIndex].EvaluateTime();
        }
    }
}
