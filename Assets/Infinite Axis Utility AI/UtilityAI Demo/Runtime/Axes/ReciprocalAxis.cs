using UnityEngine;

namespace Stirge.InfiniteAxis.Demo.Axes
{
    using GenericBlackboard;
    using Core;
    using Stirge.Serialization;
    using Tools;

    public class ReciprocalAxis : Axis, ISetupable<float, float, float, BlackboardPropertyName>
    {
        private float m_a;
        private float m_h;
        private float m_k;
        private BlackboardPropertyName m_floatPropertyName;
        
        void ISetupable<float, float, float, BlackboardPropertyName>.Setup(float a, float h, float k, BlackboardPropertyName floatPropertyName)
        {
            m_a = a;
            m_h = h;
            m_k = k;
            m_floatPropertyName = floatPropertyName;
        }
        
        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_floatPropertyName, out float value))
            {
                return m_a / (value - m_h) + m_k;
            }
            return 0;
        }
    }
}
