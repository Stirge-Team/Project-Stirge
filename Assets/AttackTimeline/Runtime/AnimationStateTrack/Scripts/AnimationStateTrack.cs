using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(AnimationStatePlayableAsset))]
[TrackBindingType(typeof(Animator))]
public class AnimationStateTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        /*
        if (m_targetAnimationStateName != null && m_targetAnimationStateName != string.Empty)
            clip.displayName = m_targetAnimationStateName;
        */
        clip.duration = 1d;
        base.OnCreateClip(clip);
    }
}
