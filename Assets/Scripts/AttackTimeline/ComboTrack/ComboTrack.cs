using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Stirge.AttackTimeline.ComboTrack
{
    using Input;
    using UtilityAI;

    [TrackClipType(typeof(ComboPlayableAsset))]
    [TrackBindingType(typeof(PlayerInputProcessing))]
    [TrackColor(1f, 165f / 255f, 0f)]
    public class ComboTrack : TrackAsset
    {
        [SerializeField] private AttackInput m_comboInput;
        [SerializeField] private SerializedAction m_serializedComboAction;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ComboMixer.Create(graph, inputCount, m_comboInput, m_serializedComboAction);
            //m_comboMixer = mixer.GetBehaviour();
            return mixer;
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            if (m_serializedComboAction != null)
                clip.displayName = m_serializedComboAction.name;
            clip.duration = 1.5d;
        }
    }
}
