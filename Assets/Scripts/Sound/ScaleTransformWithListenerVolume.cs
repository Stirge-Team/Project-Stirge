using UnityEngine;

namespace Stirge.Sound
{
    public class ScaleTransformWithListenerVolume : MonoBehaviour
    {
        private int qSamples = 1024;
        private float[] samples;
        private float rmsValue;
        //private float dbValue;
        
        private void Update()
        {
            // i did not figure this out on my own
            // https://discussions.unity.com/t/audiomixer-output-volume/134801/2

            samples = new float[qSamples];
            AudioListener.GetOutputData(samples, 0);

            float sum = 0;
            for (int i = 0; i < qSamples; i++)
                sum += samples[i] * samples[i];

            rmsValue = Mathf.Sqrt(sum / qSamples);

            /*
            dbValue = Mathf.Log10(rmsValue / 0.1f);
            if (dbValue < -160)
                dbValue = -160;
            */

            transform.localScale = Vector3.one * rmsValue;
        }
    }
}
