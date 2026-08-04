using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    public class ApproachTargetNode : AttackNode, ISetupable<RandomFloatField, bool, RandomFloatField>
    {
        private RandomFloatField m_stoppingDistance;
        private bool m_useInitialPosition = true;
        private RandomFloatField m_speed;

        public float StoppingDistance => m_stoppingDistance.Value;
        public bool UseInitialPosition => m_useInitialPosition;
        public float Speed => m_speed.Value;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_stoppingDistance.DetermineValue();
            m_speed.DetermineValue();
            activeNodes.Add(this);
        }

        public void Setup(RandomFloatField stoppingDistance, bool useInitialPosition, RandomFloatField speed)
        {
            m_stoppingDistance = stoppingDistance;
            m_useInitialPosition = useInitialPosition;
            m_speed = speed;
        }
    }
}
