using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using System.ComponentModel;
[DisplayName("Move Clip")]
#endif

[Serializable]
public class MovePlayableAsset : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] private AnimationCurve3D m_velocity;
    [SerializeField] private bool m_isLocal = true;
    
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return MoveBehaviour.Create(graph, m_velocity, m_isLocal);
    }
}
