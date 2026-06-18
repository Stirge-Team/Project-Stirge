using Stirge.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public abstract class MoveNode : AttackNode
    {
        [SerializeField] private RandomVector3Field m_localOffset = new(0);
        [SerializeField] private RandomFloatField m_stoppingDistance = new(0);
        [SerializeField] private bool m_considerYPosition = true;

        public Vector3 LocalOffset => m_localOffset.Value;
        public float StoppingDistance => m_stoppingDistance.Value;
        public bool ConsiderYPosition => m_considerYPosition;

        public override void Evaluate(List<AttackNode> activeNodes)
        {
            m_localOffset.DetermineValue();
            m_stoppingDistance.DetermineValue();
        }
    }
}
