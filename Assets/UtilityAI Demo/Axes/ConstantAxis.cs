namespace Stirge.UtilityAI.Core.Axes
{
    using Stirge.Serialization;

    public class ConstantAxis : Axis, ISetupable<float>
    {
        private float m_score;

        void ISetupable<float>.Setup(float score)
        {
            m_score = score;
        }

        public override float ComputeScore()
        {
            return m_score;
        }
    }
}
