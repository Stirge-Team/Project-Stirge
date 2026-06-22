using Stirge.Serialization;
using Stirge.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
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
