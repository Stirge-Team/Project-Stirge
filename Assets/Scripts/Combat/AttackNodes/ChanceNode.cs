using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public class ChanceNode : AttackNode
    {
        [SerializeField, Range(0, 1)] private float m_chance;
        [SerializeReference] private AttackNode m_node;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            if (Random.Range(0f, 1f) <= m_chance)
            {
                m_node.Evaluate(activeNodes);
            }
        }
        public override float EvaluateTime()
        {
            return 0f;
        }
    }
}
