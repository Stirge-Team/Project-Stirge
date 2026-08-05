using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(AnimationStatePlayableAsset))]
[TrackBindingType(typeof(Animator))]
public class AnimationStateTrack : TrackAsset
{   
    [SerializeField] private PostPlaybackState m_postPlaybackState;
    [SerializeField] private string m_targetAnimationStateName;
    [SerializeField] private string m_triggerParameterName;

    private AnimationStateMixerPlayable m_animationStateMixer;

    public PostPlaybackState postPlaybackState
    {
        get => m_postPlaybackState;
        set => m_postPlaybackState = value;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = AnimationStateMixerPlayable.Create(graph, inputCount, m_targetAnimationStateName, m_triggerParameterName);
        m_animationStateMixer = mixer.GetBehaviour();

        UpdateTrackMode();

        return mixer;
    }

    internal void UpdateTrackMode()
    {
        if (m_animationStateMixer != null)
        {
            m_animationStateMixer.postPlaybackState = m_postPlaybackState;
        }
    }

    protected override void OnCreateClip(TimelineClip clip)
    {
        if (m_targetAnimationStateName != null && m_targetAnimationStateName != string.Empty)
            clip.displayName = m_targetAnimationStateName;
        clip.duration = 1.5d;
        base.OnCreateClip(clip);
    }

    public enum PostPlaybackState
    {
        Set,
        Reset
    }
}
