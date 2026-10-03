using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline
{
#if UNITY_EDITOR
    using System.ComponentModel;
    [DisplayName("Animation State Clip")]
#endif
    [Serializable]
    public class AnimationStatePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private string m_targetAnimationStateName;
        [SerializeField] private string m_exitParameterName;

        //this clip is not serialized as it is only used for displaying previews
        private AnimationClip m_previewClip;

        public string TargetAnimationStateName
        {
            get => m_targetAnimationStateName;
            set => m_targetAnimationStateName = value;
        }

        public AnimationClip PreviewClip
        {
            get => m_previewClip;
            set => m_previewClip = value;
        }

        /// <summary>
        /// Duration of the clip
        /// </summary>
        public override double duration
        {
            get
            {
                if (m_previewClip == null)
                    return base.duration;

                double length = m_previewClip.length;
                return length > 0.001 ? length : base.duration;
            }
        }

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return AnimationStateBehaviour.Create(graph, m_targetAnimationStateName, m_exitParameterName, PreviewClip);
        }
    }
}
