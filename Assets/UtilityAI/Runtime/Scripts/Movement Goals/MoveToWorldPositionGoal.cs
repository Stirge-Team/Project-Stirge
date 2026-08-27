using Stirge.Combat;
using Stirge.Serialization;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public class MoveToWorldPositionGoal : MovementGoal, ISetupable<Vector3>
    {
        private Vector3 m_worldPosition;
        public Vector3 worldPosition => m_worldPosition;

        public void Setup(Vector3 worldPosition)
        {
            m_worldPosition = worldPosition;
        }

        protected override float EvaluateInternal(CombatEntity user)
        {
            return 1f;
        }
    }
}
