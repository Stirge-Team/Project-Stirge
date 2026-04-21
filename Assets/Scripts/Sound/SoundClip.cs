using UnityEngine;
using UnityEngine.Audio;

namespace Stirge.Sound
{
    [CreateAssetMenu(fileName = "New Sound Clip", menuName = "Stirge/Sound Clip", order = 1)]

    public class SoundClip : ScriptableObject
    {
        [Header("Audio Properties")]
        [SerializeField] private AudioResource m_audioResource;
        [SerializeField] private AudioMixerGroup m_mixerGroup;

        [Header("Playback Properties")]
        [SerializeField] private bool m_mute;
        [SerializeField] private bool m_bypassEffects;
        [SerializeField] private bool m_bypassListenerEffects;
        [SerializeField] private bool m_bypassReverbZones;
        [SerializeField] private bool m_loop;
        [SerializeField] private int m_priority;
        [SerializeField, Range(0f, 1f)] private float m_volume;
        [SerializeField, Range(-3f, 3f)] private float m_pitch;
        [SerializeField, Range(-1f, 1f)] private float m_stereoPan;
        [SerializeField, Range(0f, 1f)] private float m_spatialBlend;

        public AudioResource AudioResource => m_audioResource;
        public AudioMixerGroup MixerGroup => m_mixerGroup;

        public bool Mute => m_mute;
        public bool BypassEffects => m_bypassEffects;
        public bool BypassListenerEffects => m_bypassListenerEffects;
        public bool BypassReverbZones => m_bypassReverbZones;
        public bool Loop => m_loop;
        public int Priority => m_priority;
        public float Volume => m_volume;
        public float Pitch => m_pitch;
        public float StereoPan => m_stereoPan;
        public float SpatialBlend => m_spatialBlend;

        private void Reset()
        {
            m_audioResource = null;
            m_mixerGroup = null;

            m_mute = false;
            m_bypassEffects = false;
            m_bypassListenerEffects = false;
            m_bypassReverbZones = false;
            m_loop = false;
            m_priority = 128;
            m_volume = 1f;
            m_pitch = 1f;
            m_stereoPan = 0f;
            m_spatialBlend = 0f;
        }
    }
}
