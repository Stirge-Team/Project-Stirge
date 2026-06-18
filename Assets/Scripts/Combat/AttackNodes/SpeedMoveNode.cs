using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class SpeedMoveNode : MoveNode
    {
        [SerializeField] private RandomFloatField m_speed;

        public float Speed => m_speed.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            base.Evaluate(null);
            m_speed.DetermineValue();

            activeNodes.Add(this);
        }
    }
}
