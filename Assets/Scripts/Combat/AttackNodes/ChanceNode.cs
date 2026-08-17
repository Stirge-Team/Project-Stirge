using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;

    public class ChanceNode : DecoratorNodeSingle, ISetupable<AttackNode, float>
    {
        private float m_chance;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            if (Random.value <= m_chance)
            {
                m_node.Evaluate(activeNodes);
            }
        }

        public void Setup(AttackNode node, float chance)
        {
            base.Setup(node);
            m_chance = chance;
        }
    }
}
