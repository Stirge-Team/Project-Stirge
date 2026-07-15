using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Stirge.Serialization;
    using Tools;

    public class AbsoluteAxis : Axis, ISetupable<BlackboardPropertyName>
    {
        private BlackboardPropertyName m_floatProperty;

        void ISetupable<BlackboardPropertyName>.Setup(BlackboardPropertyName propertyName)
        {
            m_floatProperty = propertyName;
        }

        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_floatProperty, out float value))
            {
                return Scoring.GetNormalisedScore(value, 0, 1);
            }
            return 0;
        }
    }
}
