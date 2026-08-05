using Stirge.Combat;
using Stirge.Input;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// A behaviour that is attached to a playable
public class ComboMixerPlayable : PlayableBehaviour
{
    private readonly AttackInput m_comboInput;
    private readonly TimelineAsset m_comboTimeline;

    private PlayerInputProcessing m_boundPIP;
    private ComboState m_comboState;
    
    public ComboMixerPlayable() { }
    public ComboMixerPlayable(AttackInput comboInput, TimelineAsset comboTimeline)
    {
        m_comboInput = comboInput;
        m_comboTimeline = comboTimeline;
        m_comboState = ComboState.Closed;
    }

    public static ScriptPlayable<ComboMixerPlayable> Create(PlayableGraph graph, int inputCount, AttackInput comboInput, TimelineAsset comboTimeline)
    {
        return ScriptPlayable<ComboMixerPlayable>.Create(graph, new(comboInput, comboTimeline), inputCount);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_boundPIP == null)
            return;

        m_boundPIP.ClearComboBinding();
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
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
