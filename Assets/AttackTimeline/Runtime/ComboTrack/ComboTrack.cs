using Stirge.Input;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(ComboPlayableAsset))]
[TrackBindingType(typeof(PlayerInputProcessing))]
[TrackColor(1f, 165f / 255f, 0f)]
public class ComboTrack : TrackAsset
{
    [SerializeField] private AttackInput m_comboInput;
    [SerializeField] private TimelineAsset m_comboTimeline;

    private ComboMixer m_comboMixer;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ComboMixer.Create(graph, inputCount, m_comboInput, m_comboTimeline);
        m_comboMixer = mixer.GetBehaviour();

        return mixer;
    }

    protected override void OnCreateClip(TimelineClip clip)
    {
        if (m_comboTimeline != null)
            clip.displayName = m_comboTimeline.name;
        clip.duration = 1.5d;
        base.OnCreateClip(clip);
    }
}
