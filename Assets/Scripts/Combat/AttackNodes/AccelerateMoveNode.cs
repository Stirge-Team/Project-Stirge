using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    [System.Serializable]
    public class AccelerateMoveNode : MoveNode, ISetupable<RandomVector3Field, RandomFloatField, bool, RandomFloatField, RandomFloatField>
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

        public void Setup(RandomVector3Field localOffset, RandomFloatField stoppingDistance, bool considerYPosition, RandomFloatField acceleration, RandomFloatField maxSpeed)
        {
            base.Setup(localOffset, stoppingDistance, considerYPosition);
            m_acceleration = acceleration;
            m_maxSpeed = maxSpeed;
        }
    }
}
