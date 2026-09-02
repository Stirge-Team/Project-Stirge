using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Axes
{
    using GenericBlackboard;
    using Core;
    using Stirge.Serialization;

    public class LogarithmicAxis : Axis, ISetupable<float, float, float, float, BlackboardPropertyName>
    {
        private float m_a;
        private float m_b;
        private float m_h;
        private float m_k;
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
            /*
            if (Blackboard.TryGetStructValue(m_floatPropertyName, out float value))
            {
                float score = m_a * Mathf.Log(value - m_h, m_b) + m_k;
                if (!float.IsFinite(score))
                    return 0;
                return score;
            }
            */
            return 0;
        }
    }
}
