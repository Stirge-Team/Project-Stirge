using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Stirge.Serialization;
    using Tools;

    public class DistanceFromWorldPositionAxis : Axis, ISetupable<BlackboardPropertyName, Vector3, float, float, bool>
    {
        private BlackboardPropertyName m_transformPropertyName;
        private Vector3 m_worldPosition;
        private float m_lowerBounds;
        private float m_upperBounds;
        private bool m_inverted;
        
        void ISetupable<BlackboardPropertyName, Vector3, float, float, bool>.Setup(BlackboardPropertyName transformPropertyName, Vector3 worldPosition, float lowerBounds, float upperBounds, bool inverted)
        {
            m_transformPropertyName = transformPropertyName;
            m_worldPosition = worldPosition;
            m_lowerBounds = lowerBounds;
            m_upperBounds = upperBounds;
            m_inverted = inverted;
        }
        
        public override float ComputeScore()
        {
            if (Blackboard.TryGetClassValue(m_transformPropertyName, out Transform t))
            {
                float score = Scoring.GetNormalisedScore(Vector3.Distance(t.position, m_worldPosition), m_lowerBounds, m_upperBounds);
                return m_inverted ? 1f - score : score;
            }
            return 0;
        }
    }
}
