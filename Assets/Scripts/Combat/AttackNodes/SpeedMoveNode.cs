using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class SpeedMoveNode : AttackNode
    {
        [SerializeField] private RandomFloatField m_speed;
        [SerializeField] private RandomVector3Field m_localOffset;

        public float Speed => m_speed.Value;
        public Vector3 LocalOffset => m_localOffset.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_speed.DetermineValue();
            m_localOffset.DetermineValue();

            activeNodes.Add(this);
        }
    }
}
