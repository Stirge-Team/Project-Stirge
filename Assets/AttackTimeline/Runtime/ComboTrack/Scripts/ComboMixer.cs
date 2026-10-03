using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline
{
    using Input;

    public class ComboMixer : PlayableBehaviour
    {
        private readonly AttackInput m_comboInput;
        private readonly TimelineAsset m_comboTimeline;

        private PlayerInputProcessing m_boundPIP;
        private ComboState m_comboState;

        public ComboMixer() { }
        public ComboMixer(AttackInput comboInput, TimelineAsset comboTimeline)
        {
            m_comboInput = comboInput;
            m_comboTimeline = comboTimeline;
            m_comboState = ComboState.Closed;
        }

        public static ScriptPlayable<ComboMixer> Create(PlayableGraph graph, int inputCount, AttackInput comboInput, TimelineAsset comboTimeline)
        {
            return ScriptPlayable<ComboMixer>.Create(graph, new(comboInput, comboTimeline), inputCount);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_boundPIP == null)
                return;

            m_boundPIP.ClearComboBinding();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
                return;

            // checks if clip is playing
            bool HasInput()
            {
                int inputCount = playable.GetInputCount();
                for (int i = 0; i < inputCount; i++)
                {
                    if (playable.GetInputWeight(i) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }

            if (m_boundPIP == null)
            {
                m_boundPIP = playerData as PlayerInputProcessing;
            }

            if (m_boundPIP == null)
                return;

            switch (m_comboState)
            {
                case ComboState.Closed:
                    if (HasInput())
                    {
                        m_boundPIP.AddComboBinding(m_comboInput, m_comboTimeline);
                        m_comboState = ComboState.Open;
                    }
                    break;
                case ComboState.Open:
                    if (!HasInput())
                    {
                        m_boundPIP.ClearComboBinding();
                        m_comboState = ComboState.Closed;
                    }
                    break;
                default:
                    break;
            }
        }

        private enum ComboState
        {
            Closed,
            Open
        }
    }
}
