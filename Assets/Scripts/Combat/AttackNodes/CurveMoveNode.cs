using Stirge.Serialization;
using Stirge.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    [System.Serializable]
    public class CurveMoveNode : MoveNode, ISetupable<RandomVector3Field, RandomFloatField, bool, AnimationCurve>
    {
        [SerializeField] private AnimationCurve m_curve;

        public AnimationCurve Curve => m_curve;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            base.Evaluate(null);
            activeNodes.Add(this);
        }

        public void Setup(RandomVector3Field localOffset, RandomFloatField stoppingDistance, bool considerYPosition, AnimationCurve curve)
        {
            base.Setup(localOffset, stoppingDistance, considerYPosition);
            m_curve = curve;
        }
    }
}
