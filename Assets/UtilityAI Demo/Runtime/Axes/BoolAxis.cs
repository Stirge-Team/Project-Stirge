using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Blackboard;
    using Stirge.Serialization;

    public class BoolAxis : Axis, ISetupable<BlackboardPropertyName, bool>
    {
        private BlackboardPropertyName m_boolPropertyName;
        private bool m_inverted;
        
        void ISetupable<BlackboardPropertyName, bool>.Setup(BlackboardPropertyName boolPropertyName, bool inverted)
        {
            m_boolPropertyName = boolPropertyName;
            m_inverted = inverted;
        }
        
        public override float ComputeScore()
        {
            if (Blackboard.TryGetStructValue(m_boolPropertyName, out bool value))
            {
                return value != m_inverted ? 1f : 0f;
            }
            return 0f;
        }
    }
}
