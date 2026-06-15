using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [System.Serializable]
    public class CurveMoveNode : AttackNode
    {
        [SerializeField] private AnimationCurve m_curve;
        [SerializeField] private RandomVector3Field m_localOffset;

        public AnimationCurve Curve => m_curve;
        public Vector3 LocalOffset => m_localOffset.Value;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_localOffset.DetermineValue();

            activeNodes.Add(this);
        }
    }
}
