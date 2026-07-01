using UnityEngine;

namespace Stirge.UtilityAI.Core.Axes
{
    using Stirge.Combat;
    using Stirge.Serialization;

    public class AbsoluteAxis : Axis, ISetupable<string>
    {
        private string m_callbackName;

        void ISetupable<string>.Setup(string callbackName)
        {
            m_callbackName = new(callbackName);
        }

        public override float ComputeScore()
        {
            /*
            if (enemy.TryGetClassValue(m_callbackName, out FloatCallback callback))
            {
                float score = callback.Invoke();
                return Scoring.GetNormalisedScore(score, 0, 1);
            }
            */
            return 0;
        }
    }
}
