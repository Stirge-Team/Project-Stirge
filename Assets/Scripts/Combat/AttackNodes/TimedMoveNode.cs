using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class TimedMoveNode : MoveNode
    {
        [SerializeField] private RandomFloatField m_time;
        
        public float Time => m_time.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            base.Evaluate(null);
            m_time.DetermineValue();
            
            activeNodes.Add(this);
        }
    }
}
