using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    public class SequenceAttackNode : DecoratorNodeMulti
    {
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            foreach (AttackNode node in m_nodes)
            {
                node.Evaluate(activeNodes);
            }
        }
    }
}
