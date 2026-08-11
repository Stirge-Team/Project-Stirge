using Stirge.Combat;
using System;
using UnityEngine;
using UnityEngine.Timeline;

[Serializable]
[TrackClipType(typeof(MovePlayableAsset))]
[TrackBindingType(typeof(CombatEntity))]
public class MoveTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = 1.5d;
    }
}
