#if UNITY_EDITOR
using System.ComponentModel;
using Timeline.Samples;

#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline
{
#if UNITY_EDITOR
    [DisplayName("Hitbox Clip")]
#endif
    public class HitboxClip : PlayableAsset , ITimelineClipAsset
    {
        [SerializeField] private HitboxData m_data;

        public ClipCaps clipCaps { get { return ClipCaps.None; } }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<HitboxBehaviour> playable = ScriptPlayable<HitboxBehaviour>.Create(graph);
            HitboxBehaviour hitbox = playable.GetBehaviour();

            hitbox.HitboxData = m_data;

            return playable;

            //return Playable.Create(graph);
        }
    }
}

