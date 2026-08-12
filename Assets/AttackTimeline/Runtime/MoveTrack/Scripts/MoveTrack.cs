using Stirge.Combat;
using Stirge.Enemy;
using System;
using UnityEngine;
using UnityEngine.Timeline;

[Serializable]
[TrackClipType(typeof(MovePlayableAsset))]
[TrackBindingType(typeof(EnemyMotor))]
public class MoveTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = 1.5d;
    }
}
