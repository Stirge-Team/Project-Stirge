using UnityEngine;
using UnityEngine.Audio;

namespace Stirge.Sound
{
    [CreateAssetMenu(fileName = "New Sound Clip", menuName = "Stirge/Sound Clip", order = 1)]

    public class SoundClip : ScriptableObject
    {
        [SerializeField] private AudioClip m_audioClip;
        [SerializeField] private AudioMixerGroup m_mixerGroup;

        [SerializeField] private int m_priority;
        [SerializeField, Range(0f, 1f)] private float m_volume;
        [SerializeField, Range(-3f, 3f)] private float m_pitch;
        [SerializeField, Range(-1f, 1f)] private float m_stereoPan;
        [SerializeField, Range(0f, 1f)] private float m_spatialBlend;

        public AudioClip AudioClip => m_audioClip;
        public AudioMixerGroup MixerGroup => m_mixerGroup;

        public int Priority => m_priority;
        public float Volume => m_volume;
        public float Pitch => m_pitch;
        public float StereoPan => m_stereoPan;
        public float SpatialBlend => m_spatialBlend;

        private void Reset()
        {
            m_audioClip = null;
            m_mixerGroup = null;
            m_priority = 128;
            m_volume = 1f;
            m_pitch = 1f;
            m_stereoPan = 0f;
            m_spatialBlend = 0f;
        }
    }
}
