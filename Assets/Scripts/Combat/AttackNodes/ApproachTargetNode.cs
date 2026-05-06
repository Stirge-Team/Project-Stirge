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
        [SerializeField] protected RandomFloatField m_time;

        public float StoppingDistance => m_stoppingDistance.Value;
        public bool UseInitialPosition => m_useInitialPosition;
        public float Time => m_time.Value;
        
        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_stoppingDistance.DetermineValue();
            m_time.DetermineValue();
            activeNodes.Add(this);
        }

        public override float EvaluateTime()
        {
            return Time;
        }
    }
}
