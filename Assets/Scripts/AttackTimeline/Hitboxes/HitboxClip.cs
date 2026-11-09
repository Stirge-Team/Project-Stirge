using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline.HitboxTrack
{
#if UNITY_EDITOR
    [System.ComponentModel.DisplayName("Hitbox Clip")]
#endif
    public class HitboxClip : PlayableAsset , ITimelineClipAsset
    {
        [SerializeField] private HitboxBehaviour m_template;

        public ClipCaps clipCaps { get { return ClipCaps.None; } }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<HitboxBehaviour> playable = ScriptPlayable<HitboxBehaviour>.Create(graph, m_template);
            //HitboxBehaviour clone = playable.GetBehaviour();
            return playable;
        }
    }
}

