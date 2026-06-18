using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class CurveMoveNode : MoveNode
    {
        [SerializeField] private AnimationCurve m_curve;

        public AnimationCurve Curve => m_curve;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            base.Evaluate(null);
            activeNodes.Add(this);
        }
    }
}
