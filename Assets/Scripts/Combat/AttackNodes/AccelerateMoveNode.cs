using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class AccelerateMoveNode : AttackNode
    {
        [SerializeField] private RandomFloatField m_acceleration;
        [SerializeField] private RandomFloatField m_maxSpeed;
        [SerializeField] private RandomVector3Field m_localOffset;

        public float Acceleration => m_acceleration.Value;
        public float MaxSpeed => m_maxSpeed.Value;
        public Vector3 LocalOffset => m_localOffset.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_acceleration.DetermineValue();
            m_maxSpeed.DetermineValue();
            m_localOffset.DetermineValue();

            activeNodes.Add(this);
        }
    }
}
