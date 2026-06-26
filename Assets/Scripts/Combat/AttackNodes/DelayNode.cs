using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    [System.Serializable]
    public class DelayNode : AttackNode, ISetupable<RandomFloatField>
    {
        private RandomFloatField m_delay;

        public float Delay => m_delay.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_delay.DetermineValue();
            activeNodes.Add(this);
        }

        void ISetupable<RandomFloatField>.Setup(RandomFloatField arg)
        {
            m_delay = arg;
        }
    }
}
