using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Stirge.Camera
{
    public class CameraShakeController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineBasicMultiChannelPerlin[] m_shakables;
        private Coroutine[] m_shakeCorountine = new Coroutine[0];

        private static CameraShakeController m_instance;
        public static CameraShakeController Instance => m_instance;

        void Start()
        {
            //singlton
            if (m_instance == null)
            {
                m_instance = this;
            }
            else if (m_instance != this)
            {
                Debug.LogWarning("There are too many Camera shake controllers in the scene! Please ensure that there is only 1 camera shake controller!");
                enabled = false;
                return;
            }

            m_shakeCorountine = new Coroutine[m_shakables.Length];
        }
        public void BeginScreenshake()
        {
            BeginScreenshake(new(10, 1, 10));
        }

        public void BeginScreenshake(CameraShakePreset preset)
        {
            for (int i = 0; i < m_shakables.Length; i++) m_shakeCorountine[i] = StartCoroutine(ScreenshakeCoroutine(m_shakables[i], preset));
        }
        private IEnumerator ScreenshakeCoroutine(CinemachineBasicMultiChannelPerlin shaker, CameraShakePreset preset)
        {
            Debug.Log($"Beginning screen shake on {shaker.name} with an amplitude of {preset.AmplitudeGain} and frequency of {preset.FrequencyGain} and a time of {preset.Duration} second(s).");
            shaker.AmplitudeGain = preset.AmplitudeGain; //set
            shaker.FrequencyGain = preset.FrequencyGain;

            float remainingTime = preset.Duration;
            while (remainingTime > 0)
            {
                remainingTime -= Time.unscaledDeltaTime;//Time.deltaTime;
                var scale = preset.StrengthOverTime.Evaluate(1 - (remainingTime / preset.Duration));
                shaker.AmplitudeGain = preset.AmplitudeGain * scale; //reduce by rate
                shaker.FrequencyGain = preset.FrequencyGain * scale;
                yield return null;
            }

            Debug.Log("Shaking has run out! Setting both gains to 0.");
            shaker.AmplitudeGain = shaker.FrequencyGain = 0; //reset
        }
        public void StopScreenshake(int targetCamera = -1)
        {
            if (targetCamera >= 0 && targetCamera < m_shakables.Length)
            {
                StopCoroutine(m_shakeCorountine[targetCamera]);
            }
            else
            {
                StopAllCoroutines();
            }
        }
        public void StopScreenshake(int[] targetCameras)
        {
            foreach (int index in targetCameras)
            {
                if (index >= 0 && index < m_shakables.Length)
                {
                    StopCoroutine(m_shakeCorountine[index]);
                }
            }
        }
    }
}
