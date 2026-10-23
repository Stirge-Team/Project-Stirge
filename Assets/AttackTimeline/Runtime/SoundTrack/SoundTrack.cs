using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline.SoundTrack
{
    [Serializable]
    [TrackClipType(typeof(SoundTrackClip))]
    public class SoundTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.duration = 0.5f;
            base.OnCreateClip(clip);
        }
    }
}
