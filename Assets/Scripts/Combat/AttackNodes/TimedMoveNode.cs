using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class TimedMoveNode : AttackNode
    {
        [SerializeField] private RandomFloatField m_time;
        [SerializeField] private RandomVector3Field m_localOffset;

        public float Time => m_time.Value;
        public Vector3 LocalOffset => m_localOffset.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_time.DetermineValue();
            m_localOffset.DetermineValue();
            
            activeNodes.Add(this);
        }
    }
}
