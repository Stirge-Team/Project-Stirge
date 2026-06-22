using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    [System.Serializable]
    public class SpeedMoveNode : MoveNode, ISetupable<RandomVector3Field, RandomFloatField, bool, RandomFloatField>
    {
        [SerializeField] private RandomFloatField m_speed;

        public float Speed => m_speed.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            base.Evaluate(null);
            m_speed.DetermineValue();

            activeNodes.Add(this);
        }

        public void Setup(RandomVector3Field localOffset, RandomFloatField stoppingDistance, bool considerYPosition, RandomFloatField speed)
        {
            base.Setup(localOffset, stoppingDistance, considerYPosition);
            m_speed = speed;
        }
    }
}
