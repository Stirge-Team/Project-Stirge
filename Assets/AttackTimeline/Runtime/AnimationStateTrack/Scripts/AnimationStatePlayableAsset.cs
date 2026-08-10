using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;
using System.Collections.Generic;



#if UNITY_EDITOR
using System.ComponentModel;
[DisplayName("Animation State Clip")]
#endif
[Serializable]
public class AnimationStatePlayableAsset : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] private string m_targetAnimationStateName;
    [SerializeField] private string m_exitParameterName;

    //this clip is not serialized as it is only used for displaying previews
    private AnimationClip m_previewClip;

    public string TargetAnimationStateName
    {
        get => m_targetAnimationStateName;
        set => m_targetAnimationStateName = value;
    }

    public AnimationClip PreviewClip
    {
        get => m_previewClip;
        set => m_previewClip = value;
    }

    /// <summary>
    /// Duration of the clip
    /// </summary>
    public override double duration
    {
        get
        {
            if (m_previewClip == null)
                return base.duration;

            double length = m_previewClip.length;
            return length > 0.001 ? length : base.duration;
        }
    }

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        if (!Application.isPlaying)
        {
            if (m_previewClip != null && !m_previewClip.legacy)
            {
                // Create the animation clip playable
                var clipPlayable = AnimationClipPlayable.Create(graph, m_previewClip);

                //clipPlayable.SetApplyFootIK(false);

                return clipPlayable;
            }
            return Playable.Null;
        }

        return AnimationStateBehaviour.Create(graph, m_targetAnimationStateName, m_exitParameterName);
    }

    /// <summary>
    /// Outputs for this playable
    /// </summary>
    public override IEnumerable<PlayableBinding> outputs
    {
        get { yield return AnimationPlayableBinding.Create(name, this); }
    }
}
