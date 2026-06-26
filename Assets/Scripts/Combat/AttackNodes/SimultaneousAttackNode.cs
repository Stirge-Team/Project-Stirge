using UnityEngine;
using System.Collections.Generic;
using Stirge.Serialization;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class SimultaneousAttackNode : DecoratorNodeMulti, ISetupable<int, AttackNode[]>
    {
        public SimultaneousAttackNode()
        {
            m_nodes = new AttackNode[0];
        }

        [SerializeField] private int m_significantAttackNodeIndex = -1;

        public int SignificantAttackNodeIndex => m_significantAttackNodeIndex;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            activeNodes.Add(this);
        }

        public void Setup(int significantNodeIndex, AttackNode[] nodes)
        {
            m_significantAttackNodeIndex = significantNodeIndex;
            base.Setup(nodes);
        }
    }
}
