using UnityEngine;

namespace Stirge.Sound
{
    public class SoundSource : MonoBehaviour
    {
        [SerializeField] private AudioSource m_audioSource;
        private SoundClip m_soundClip;

        public SoundClip SoundClip => m_soundClip;

        public bool IsPlaying => m_audioSource.isPlaying;
        public bool IsLooping => m_audioSource.loop;

        public void PlaySound(SoundClip soundClip)
        {
            m_soundClip = soundClip;
            
            m_audioSource.resource = soundClip.AudioResource;
            m_audioSource.outputAudioMixerGroup = soundClip.MixerGroup;

            m_audioSource.mute = soundClip.Mute;
            m_audioSource.bypassEffects = soundClip.BypassEffects;
            m_audioSource.bypassListenerEffects = soundClip.BypassListenerEffects;
            m_audioSource.bypassReverbZones = soundClip.BypassReverbZones;
            m_audioSource.loop = soundClip.Loop;
            m_audioSource.priority = soundClip.Priority;
            m_audioSource.volume = soundClip.Volume;
            m_audioSource.pitch = soundClip.Pitch;
            m_audioSource.panStereo = soundClip.StereoPan;
            m_audioSource.spatialBlend = soundClip.SpatialBlend;

            m_audioSource.Play();
        }

        public void Stop()
        {
            m_audioSource.Stop();
        }
    }
}
