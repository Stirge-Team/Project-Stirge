using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline.ComboTrack
{
    using Input;
    using UtilityAI;

    public class ComboMixer : PlayableBehaviour
    {
        private readonly AttackInput m_comboInput;
        private readonly SerializedAction m_comboAction;

        private PlayerInputProcessing m_boundPIP;
        private ComboState m_comboState;

        public ComboMixer() { }
        public ComboMixer(AttackInput comboInput, SerializedAction comboAction)
        {
            m_comboInput = comboInput;
            m_comboAction = comboAction;
            m_comboState = ComboState.Closed;
        }

        public static ScriptPlayable<ComboMixer> Create(PlayableGraph graph, int inputCount, AttackInput comboInput, SerializedAction comboAction)
        {
            return ScriptPlayable<ComboMixer>.Create(graph, new(comboInput, comboAction), inputCount);
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
                        m_boundPIP.AddComboBinding(m_comboInput, m_comboAction);
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
