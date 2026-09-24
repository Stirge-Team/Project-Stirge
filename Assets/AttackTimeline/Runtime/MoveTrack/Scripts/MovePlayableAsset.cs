using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline
{
    using Tools;

#if UNITY_EDITOR
    using System.ComponentModel;
    [DisplayName("Move Clip")]
#endif
    [Serializable]
    public class MovePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private AnimationCurve3D m_translation;
        [SerializeField] private bool m_isLocal = true;

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return MoveBehaviour.Create(graph, m_translation, m_isLocal);
        }
    }
}
