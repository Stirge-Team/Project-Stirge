using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline.StatusTrack
{
#if UNITY_EDITOR
    using System.ComponentModel;
    [DisplayName("Status Clip")]
#endif
    [System.Serializable]
    public class StatusClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private StatusBehaviour m_template;

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return StatusBehaviour.Create(graph, m_template);
        }
    }
}
