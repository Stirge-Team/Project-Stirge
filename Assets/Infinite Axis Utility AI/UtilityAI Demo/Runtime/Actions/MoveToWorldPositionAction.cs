using UnityEngine;

namespace Stirge.InfiniteAxis.Core.Actions
{
    using GenericBlackboard;
    using Stirge.Serialization;

    public class MoveToWorldPositionAction : Action, ISetupable<Vector3, BlackboardPropertyName>
    {
        private Vector3 m_worldPosition;
        private BlackboardPropertyName m_targetPositionProperty;
        
        void ISetupable<Vector3, BlackboardPropertyName>.Setup(Vector3 worldPosition, BlackboardPropertyName targetPositionProperty)
        {
            m_worldPosition = worldPosition;
            m_targetPositionProperty = targetPositionProperty;
        }
        
        protected override void OnBegin()
        {
            Blackboard.SetStructValue(m_targetPositionProperty, m_worldPosition);
        }
    }
}
