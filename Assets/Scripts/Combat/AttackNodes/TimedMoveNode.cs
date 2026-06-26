using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Stirge.Serialization;
    using Tools;

    [System.Serializable]
    public class TimedMoveNode : MoveNode, ISetupable<RandomVector3Field, RandomFloatField, bool, RandomFloatField>
    {
        [SerializeField] private RandomFloatField m_time;
        
        public float Time => m_time.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            base.Evaluate(null);
            m_time.DetermineValue();
            
            activeNodes.Add(this);
        }

        public void Setup(RandomVector3Field localOffset, RandomFloatField stoppingDistance, bool considerYPosition, RandomFloatField time)
        {
            base.Setup(localOffset, stoppingDistance, considerYPosition);
            m_time = time;
        }
    }
}
