using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Axes
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;

    public class QuadraticAxis : Axis, ISetupable<float, float, float, BlackboardPropertyName>
    {
        private float m_a;
        private float m_b;
        private float m_c;
        private BlackboardPropertyName m_floatPropertyName;
        
        void ISetupable<float, float, float, BlackboardPropertyName>.Setup(float a, float b, float c, BlackboardPropertyName floatPropertyName)
        {
            m_a = a;
            m_b = b;
            m_c = c;
            m_floatPropertyName = floatPropertyName;
        }

        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_floatPropertyName, out float value))
            {
                return m_a * value * value + m_b * value + m_c;
            }
            return 0;
        }
    }
}
