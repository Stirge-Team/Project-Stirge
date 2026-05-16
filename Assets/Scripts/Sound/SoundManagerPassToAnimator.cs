using System;
using UnityEngine;

namespace Stirge.Sound
{
    public class SoundManagerPassToAnimator : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (SoundManager.Instance == null)
            {
                Debug.LogError($"Sound Manager not found. Please check that the sound manager isn't disabled, add the sound manager to your scene or remove this script from {name}");
            }
            else Debug.Log($"Sound Manager found as object {SoundManager.Instance}");
        }
        public void PlayClip(SoundClip clip)
        {
            SoundManager.Instance.PlaySoundClipOnObject(clip, transform);
        }
    }
}
