using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Stirge.Serialization;
    using Tools;

    public class ClampedAbsoluteAxis : Axis, ISetupable<BlackboardPropertyName, float, float, bool>
    {
        private BlackboardPropertyName m_propertyName;
        private float m_lowerBound;
        private float m_upperBound;
        private bool m_inverted;
        
        void ISetupable<BlackboardPropertyName, float, float, bool>.Setup(BlackboardPropertyName propertyName, float lowerBound, float upperBound, bool inverted)
        {
            m_propertyName = propertyName;
            m_lowerBound = lowerBound;
            m_upperBound = upperBound;
            m_inverted = inverted;
        }
        
        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_propertyName, out float value))
            {
                float score = Scoring.GetNormalisedScore(Mathf.Abs(value), m_lowerBound, m_upperBound);
                return m_inverted ? 1 - score : score;
            }
            return 0;
        }
    }
}
