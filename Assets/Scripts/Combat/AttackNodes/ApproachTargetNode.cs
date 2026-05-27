using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class ApproachTargetNode : AttackNode
    {
        [SerializeField] private RandomFloatField m_stoppingDistance;
        [SerializeField] private bool m_useInitialPosition = true;
        [SerializeField] protected RandomFloatField m_speed;

        public float StoppingDistance => m_stoppingDistance.Value;
        public bool UseInitialPosition => m_useInitialPosition;
        public float Speed => m_speed.Value;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_stoppingDistance.DetermineValue();
            m_speed.DetermineValue();
            activeNodes.Add(this);
        }
    }
}
