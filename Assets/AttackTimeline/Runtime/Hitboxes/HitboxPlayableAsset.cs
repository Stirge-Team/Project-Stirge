#if UNITY_EDITOR
using System.ComponentModel;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline
{
#if UNITY_EDITOR
    [DisplayName("Hitbox Clip")]
#endif
    public class HitboxPlayableAsset : PlayableAsset , ITimelineClipAsset
    {
        [SerializeField] private HitboxData m_data;

        public ClipCaps clipCaps { get { return ClipCaps.None; } }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }
}

