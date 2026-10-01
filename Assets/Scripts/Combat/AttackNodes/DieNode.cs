using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    public class DieNode : AttackNode, ISetupable<RandomFloatField>
    {
        private RandomFloatField m_delay;

        public float Delay => m_delay.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_delay.DetermineValue();
            
            activeNodes.Add(this);
        }

        public void Setup(RandomFloatField delay)
        {
            m_delay = delay;
        }
    }
}
