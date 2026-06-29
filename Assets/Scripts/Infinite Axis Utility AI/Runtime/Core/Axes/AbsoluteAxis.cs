using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Stirge.Serialization;

    public class AbsoluteAxis : Axis, ISetupable<string>
    {
        private string m_getValue;

        void ISetupable<string>.Setup(string getValue)
        {
            m_getValue = getValue;
        }

        public override float ComputeScore()
        {
            //if (Blackboard.TryGetClassValue<Serialized;
            return 0;
        }
    }
}
