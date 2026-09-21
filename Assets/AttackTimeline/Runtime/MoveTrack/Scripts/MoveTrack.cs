using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline
{
    using Combat;

    [Serializable]
    [TrackClipType(typeof(MovePlayableAsset))]
    [TrackBindingType(typeof(CombatEntityMotor))]
    public class MoveTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.duration = 1.5d;
        }
    }
}
