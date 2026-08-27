using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Stirge.AttackTimeline
{
    [Serializable]
    [TrackClipType(typeof(HitboxClip))]
    [TrackBindingType(typeof(GameObject))]
    [TrackColor(0.6901960784313725f, 0.0431372549019608f, 0.4117647058823529f)] //b00b69
    public class HitboxTrack : TrackAsset
    {
        HitboxMixerBehaviour m_HitboxMixer;

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = HitboxMixerBehaviour.Create(graph, inputCount);
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
