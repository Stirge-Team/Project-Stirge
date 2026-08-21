using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Stirge.AttackTimeline
{
    [Serializable]
    [TrackClipType(typeof(HitboxPlayableAsset))]
    [TrackBindingType(typeof(GameObject))]
    public class HitboxTrack : TrackAsset
    {
        HitboxMixerPlayable m_HitboxMixer;

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = HitboxMixerPlayable.Create(graph, inputCount);
            m_HitboxMixer = mixer.GetBehaviour();

            return mixer;
        }

        /// <inheritdoc/>
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Hitbox";
            base.OnCreateClip(clip);
        }
    }
}
