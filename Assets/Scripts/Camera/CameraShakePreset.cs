using UnityEngine;

namespace Stirge.Camera
{
    [CreateAssetMenu(fileName = "NewCameraShakePreset", menuName = "Stirge/CameraShakePreset", order = 1)]
    public class CameraShakePreset : ScriptableObject
    {
        [HideInInspector]
        public float AmplitudeGain => m_amplitude;
        [SerializeField,Tooltip("Gain to apply to the amplitudes defined in the Noise Settings asset on the Camera Shake Controller. 1 is normal. Setting this to 0 completely mutes the noise")]
        private float m_amplitude;
        [HideInInspector]
        public float FrequencyGain => m_frequency;
        [SerializeField, Tooltip("Scale factor to apply to the freqencies defined in the Noise Settings assets on the Camera Shake Controller. 1 is normal. Larger magnitudes will make the noise shake more rapidly.")]
        private float m_frequency;
        [HideInInspector]
        public float Duration => m_duration;
        [SerializeField, Min(0), Tooltip("The time - in seconds - this shake effect should last.")]
        private float m_duration;
        [HideInInspector]
        public AnimationCurve StrengthOverTime => m_strengthOverTime; 
        [SerializeField, Tooltip("How strong the amplitude and frequency should be over the duration of this shake effect. Mutliplies their values as time goes on.")]
        private AnimationCurve m_strengthOverTime = new(new(0,1),new(1,0));

        public CameraShakePreset(float amp, float freq, float duration)
        {
            m_amplitude = amp;
            m_frequency = freq;
            m_duration = duration;
            m_strengthOverTime = new(new(0,1),new(1,0));
        }
    }
}