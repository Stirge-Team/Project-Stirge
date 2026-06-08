using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    public class AbsoluteAxis : Axis, ISetupable<string>
    {
        private string m_getValue;

        void ISetupable<string>.Setup(string getValue)
        {
            m_getValue = getValue;
        }

        public override float ComputeScore()
        {
            return Blackboard.GetFloatValue(m_getValue);
        }
    }
}
