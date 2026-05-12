using UnityEngine;

namespace Stirge.Combat.Attacks
{
    using System.Collections.Generic;
    using Tools;

    public class DelayNode : AttackNode
    {
        [SerializeField] private RandomFloatField m_delay;

        public float Delay => m_delay.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_delay.DetermineValue();
            activeNodes.Add(this);
        }

        public override float EvaluateTime()
        {
            return m_delay.Value;
        }
    }
}
