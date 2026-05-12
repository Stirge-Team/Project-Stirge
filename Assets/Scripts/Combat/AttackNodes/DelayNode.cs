using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class DelayNode : AttackNode
    {
        [SerializeField] private RandomFloatField m_delay;

        public float Delay => m_delay.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_delay.DetermineValue();
            activeNodes.Add(this);
        }
    }
}
