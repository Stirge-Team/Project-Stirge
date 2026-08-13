using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Axes
{
    using Blackboard;
    using Core;
    using Stirge.Serialization;

    public class ExponentialAxis : Axis, ISetupable<float, float, float, float, BlackboardPropertyName>
    {
        private float m_a = 1f;
        private float m_b = 2f;
        private float m_h = 0f;
        private float m_k = 0f;
        private BlackboardPropertyName m_floatPropertyName;
        
        void ISetupable<float, float, float, float, BlackboardPropertyName>.Setup(float a, float b, float h, float k, BlackboardPropertyName floatPropertyName)
        {
            m_a = a;
            m_b = b;
            m_h = h;
            m_k = k;
            m_floatPropertyName = floatPropertyName;
        }
        
        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_floatPropertyName, out float value))
            {
                return m_a * Mathf.Pow(m_b, value - m_h) + m_k;
            }
            return 0;
        }
    }
}
