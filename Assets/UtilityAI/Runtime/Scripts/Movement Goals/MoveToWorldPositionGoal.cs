using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using Serialization;

    public class MoveToWorldPositionGoal : MovementGoal, ISetupable<Vector3>
    {
        private Vector3 m_worldPosition;

        public void Setup(Vector3 worldPosition)
        {
            m_worldPosition = worldPosition;
        }

        protected override float EvaluateInternal(CombatEntity user, CombatEntity target)
        {
            return Mathf.Min(3f, Vector3.Distance(user.GetPosition(), m_worldPosition));
        }
    }
}
