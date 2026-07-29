using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    public class SelectAttackNode : DecoratorNodeMulti
    {
        private int m_chosenIndex = -1;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_chosenIndex = Random.Range(0, m_nodes.Length);
            m_nodes[m_chosenIndex].Evaluate(activeNodes);
        }
    }
}
