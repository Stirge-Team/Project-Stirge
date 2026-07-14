using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Stirge.Combat;
    using Stirge.Serialization;

    public class AbsoluteAxis : Axis, ISetupable<BlackboardPropertyName>
    {
        private AxisDelegate<float> m_floatDelegate;

        void ISetupable<BlackboardPropertyName>.Setup(BlackboardPropertyName propertyName)
        {
            m_floatDelegate = new(propertyName);
        }

        public override float ComputeScore()
        {
            float value = m_floatDelegate.GetValue(enemy);
            return Scoring.GetNormalisedScore(value, 0, 1);
        }
    }
}
