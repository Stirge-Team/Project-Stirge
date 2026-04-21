using UnityEngine;
using System.Collections.Generic;

namespace Stirge.Sound
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager s_instance;
        public static SoundManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    FindFirstObjectByType<SoundManager>().Init();
                }
                return s_instance;
            }
        }
        
        [SerializeField] private List<SoundSource> m_freeSources = new();
        private List<SoundSource> m_usedSources = new();

        #region Unity Events
        private void Init()
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            // check for any Sound Sources that have finished playing and move them back to the Available Sources
            if (m_usedSources.Count != 0)
            {
                List<SoundSource> toStop = new();
                foreach (SoundSource source in m_usedSources)
                {
                    // if the source has stopped playing and the source should not loop
                    if (!source.IsPlaying && !source.IsLooping)
                    {
                        toStop.Add(source);
                    }
                }

                // update the free and used lists and reset position and parent of Audio Sources
                foreach (SoundSource source in toStop)
                {
                    StopSoundSource(source);
                }
            }
        }

        public void BeforeSceneLoad()
        {
            StopPlayingAllSounds();
        }
        #endregion

        #region Playback
        private void PlaySoundClip(SoundClip soundClip, SoundSource soundSource)
        {
            soundSource.PlaySound(soundClip);
        }

        private void StopSoundSource(SoundSource soundSource)
        {
            soundSource.Stop();
            soundSource.transform.SetParent(transform);
            soundSource.transform.localPosition = Vector3.zero;
            m_usedSources.Remove(soundSource);
            m_freeSources.Add(soundSource);
        }

        private SoundSource GetFreeAudioSource(SoundClip soundClip)
        {
            // check there are available Audio Sources
            if (m_freeSources.Count == 0)
            {
                Debug.LogError($"No Audio Sources available to play '{soundClip.name}' SoundClip.", this);
                return null;
            }
            SoundSource freeSource = m_freeSources[0];
            m_freeSources.RemoveAt(0);
            m_usedSources.Add(freeSource);
            return freeSource;
        }

        public void PlaySoundClipAtPosition(SoundClip soundClip, Vector3 position)
        {
            SoundSource source = GetFreeAudioSource(soundClip);
            if (source != null)
            {
                source.transform.position = position;
                PlaySoundClip(soundClip, source);
            }
        }

        public void PlaySoundClipOnObject(SoundClip soundClip, Transform target)
        {
            SoundSource source = GetFreeAudioSource(soundClip);
            if (source != null)
            {
                source.transform.SetParent(target);
                source.transform.localPosition = Vector3.zero;
                PlaySoundClip(soundClip, source);
            }
        }

        public void StopPlayingSoundClip(SoundClip soundClip)
        {
            // stop if no sounds playing
            if (m_usedSources.Count == 0)
            {
                Debug.LogWarning("No Sounds are currently playing, nothing to Stop");
                return;
            }
            
            // find all instances of the provided Sound Clip
            SoundSource[] toStop = m_usedSources.FindAll(source => source.SoundClip.name == soundClip.name).ToArray();
            if (toStop.Length == 0)
            {
                Debug.LogWarning($"Sound Clip '{soundClip.name}' is not currently playing.");
            }
            else
            {
                // stop each SoundClip from playing and free-up the Sound Source
                foreach (SoundSource source in toStop)
                {
                    StopSoundSource(source);
                }
            }
        }

        public void StopPlayingAllSounds()
        {
            // stop all playing Sound Sources
            if (m_usedSources.Count == 0)
                return;

            SoundSource[] toStop = m_usedSources.ToArray();

            // stop all sources from playing
            foreach (SoundSource source in toStop)
            {
                StopSoundSource(source);
            }
        }
        #endregion
    }
}
