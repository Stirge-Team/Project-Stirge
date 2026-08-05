using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using System.ComponentModel;
[DisplayName("Animation State Clip")]
#endif
[Serializable]
public class AnimationStatePlayableAsset : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] private string m_targetAnimationStateName;
    [SerializeField] private string m_exitParameterName;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return AnimationStateBehaviour.Create(graph, m_targetAnimationStateName, m_exitParameterName);
    }
}
