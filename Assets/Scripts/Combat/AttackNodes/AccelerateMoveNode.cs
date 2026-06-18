using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class AccelerateMoveNode : MoveNode
    {
        [SerializeField] private RandomFloatField m_acceleration;
        [SerializeField] private RandomFloatField m_maxSpeed;

        public float Acceleration => m_acceleration.Value;
        public float MaxSpeed => m_maxSpeed.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            base.Evaluate(null);
            m_acceleration.DetermineValue();
            m_maxSpeed.DetermineValue();

            activeNodes.Add(this);
        }
    }
}
