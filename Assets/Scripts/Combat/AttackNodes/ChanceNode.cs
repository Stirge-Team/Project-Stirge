using UnityEngine;
using System.Collections.Generic;
using Stirge.Serialization;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class ChanceNode : DecoratorNodeSingle, ISetupable<AttackNode, float>
    {
        [SerializeField, Range(0, 1)] private float m_chance;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            if (Random.Range(0f, 1f) <= m_chance)
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
