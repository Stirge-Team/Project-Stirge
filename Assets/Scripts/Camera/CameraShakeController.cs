using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Stirge.Camera
{
    public class CameraShakeController : MonoBehaviour
    {
        [Header("Global Settings")]
        [SerializeField]
        private NoiseSettings m_noisePattern;
        [SerializeField]
        private Vector3 m_pivotOffset;
        private CinemachineStateDrivenCamera m_stateCamera;
        private Coroutine m_shakeCorountine;

        void Start()
        {
            m_stateCamera = GetComponentInChildren<CinemachineStateDrivenCamera>();
            if (m_stateCamera != null) Debug.Log($"Found {m_stateCamera.name} as the state driven camera!", this);
            else { Debug.LogError($"Failed to find a state driven camera. Removin the camera shaker.", this); enabled = false; return; }

            CinemachineCamera[] CMCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
            if (CMCams.Length == 0) { Debug.LogError("No Cinemachine cameras found! Aborting", this); enabled = false; return; }

            foreach (var cam in CMCams)
            {
                //Create and attach the shake noise to the cameras.
                CinemachineBasicMultiChannelPerlin perlin = cam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
                perlin.NoiseProfile = m_noisePattern;
                perlin.PivotOffset = m_pivotOffset;
                perlin.AmplitudeGain = 0;
                perlin.FrequencyGain = 0;
            }
            //BeginScreenshake(1, 1, 0.1f);
        }
        public void BeginScreenshake()
        {
            BeginScreenshake(new(10, 1, 10));
        }

        public void BeginScreenshake(CameraShakePreset preset)
        {
            Debug.Log("Looking for a camera to shake...");
            int index = -1;
            switch (CameraStateManager.Instance.State) //getting the current active camera
            {
                case "Explore":
                    index = 0;
                    break;
                case "Combat":
                    index = 1;
                    break;
                case "LockOn":
                    index = 2;
                    break;
            }

            if (index < 0) { Debug.LogWarning("No valid camera for the current state found!"); return; } // if not found then abort

            var perlin = m_stateCamera.ChildCameras[index].GetComponent<CinemachineBasicMultiChannelPerlin>(); // getting that camera's perlin noise
            Debug.Log($"Found the perlin component of {perlin.name}. Getting jiggy...");

            m_shakeCorountine = StartCoroutine(ScreenshakeCoroutine(perlin, preset));
        }
        private IEnumerator ScreenshakeCoroutine(CinemachineBasicMultiChannelPerlin perlin, CameraShakePreset preset)
        {
            Debug.Log($"Beginning screen shake on {perlin.name} with an amplitude of {preset.AmplitudeGain} and frequency of {preset.FrequencyGain} and a time of {preset.Duration} second(s).");
            perlin.AmplitudeGain = preset.AmplitudeGain; //set
            perlin.FrequencyGain = preset.FrequencyGain;

            float remainingTime = preset.Duration;
            while (remainingTime > 0)
            {
                remainingTime -= Time.unscaledDeltaTime;//Time.deltaTime;
                perlin.AmplitudeGain = preset.AmplitudeGain * (remainingTime / preset.Duration); //reduce by rate
                perlin.FrequencyGain = preset.FrequencyGain * (remainingTime / preset.Duration);
                yield return null;
            }

            Debug.Log("Shaking has run out! Setting both gains to 0.");
            perlin.AmplitudeGain = perlin.FrequencyGain = 0; //reset
        }
    }
}
