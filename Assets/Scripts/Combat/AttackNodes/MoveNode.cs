using Stirge.Serialization;
using Stirge.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public abstract class MoveNode : AttackNode, ISetupable<RandomVector3Field, RandomFloatField, bool>
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

        public void Setup(RandomVector3Field localOffset, RandomFloatField stoppingDistance, bool considerYPosition)
        {
            m_localOffset = localOffset;
            m_stoppingDistance = stoppingDistance;
            m_considerYPosition = considerYPosition;
        }
    }
}
