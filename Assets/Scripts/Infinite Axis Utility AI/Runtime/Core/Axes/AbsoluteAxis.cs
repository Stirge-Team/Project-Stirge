namespace Stirge.UtilityAI.Core.Axes
{
    public class AbsoluteAxis : Axis, ISetupable<SerializableCallback<float>>
    {
        private SerializableCallback<float> m_getValue;

        void ISetupable<SerializableCallback<float>>.Setup(SerializableCallback<float> getValue)
        {
            m_getValue = getValue;
        }

        public override float ComputeScore()
        {
            return m_getValue.Invoke();
        }
    }
}
