using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;

    public class SimultaneousAttackNode : DecoratorNodeMulti, ISetupable<int, AttackNode[]>
    {
        private int m_significantAttackNodeIndex = -1;

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
