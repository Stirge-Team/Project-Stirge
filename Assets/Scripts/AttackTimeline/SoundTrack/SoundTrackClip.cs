using Stirge.Sound;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline.SoundTrack
{
    #if UNITY_EDITOR
    [System.ComponentModel.DisplayName("Sound clip")]
    #endif
    public class SoundTrackClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private SoundTrackBehaviour m_template;
        public ClipCaps clipCaps { get { return ClipCaps.None; }}
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<SoundTrackBehaviour> playable = ScriptPlayable<SoundTrackBehaviour>.Create(graph, m_template);
            return playable;
        }
    }
}
