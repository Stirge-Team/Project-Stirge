using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Blackboard;
    using Stirge.Serialization;

    public class LinearAxis : Axis, ISetupable<float, float, float, BlackboardPropertyName>
    {
        private float m_slope;
        private float m_horizontalShift;
        private float m_verticalShift;
        private BlackboardPropertyName m_floatPropertyName;
        
        void ISetupable<float, float, float, BlackboardPropertyName>.Setup(float slope, float horizontalShift, float verticalShift, BlackboardPropertyName floatPropertyName)
        {
            m_slope = slope;
            m_horizontalShift = horizontalShift;
            m_verticalShift = verticalShift;
            m_floatPropertyName = floatPropertyName;
        }
        
        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_floatPropertyName, out float value))
            {
                return m_slope * (value - m_horizontalShift) + m_verticalShift;
            }
            return 0;
        }
    }
}
