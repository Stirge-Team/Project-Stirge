using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline.StatusTrack
{
    using Combat;

    [TrackClipType(typeof(StatusClip))]
    [TrackBindingType(typeof(CombatEntity))]
    public class StatusTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.duration = 0.166d;
        }
    }
}
