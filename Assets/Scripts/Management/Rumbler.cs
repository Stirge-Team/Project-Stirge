using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stirge.Management
{
    public class Rumbler : MonoBehaviour
    {
        public static Rumbler Instance => m_instance;
        private static Rumbler m_instance;
        private Coroutine m_haltCoroutine;

        void Awake()
        {
            if (m_instance == null) //if null, attempt to become the new instance
            {
                m_instance = this;
                Gamepad.current.ResumeHaptics();
            }
            else if (m_instance.name != name)
            {
                Debug.LogWarning("There are multiple rumblers in scene. There should only be one rumbler, please remove any other instances!");
            }
        }

        public void RumblePulse(float lowHz, float highHz, float time)
        {
            var pad = Gamepad.current;

            if(pad != null)
            {
                Debug.Log($"Setting the rumble to {lowHz} and {highHz} for {time} seconds.");
                pad.SetMotorSpeeds(lowHz, highHz);

                m_haltCoroutine = StartCoroutine(StopRumble(time, pad));
            }
        }
        private IEnumerator StopRumble(float time, Gamepad pad)
        {
            float timePassed = 0f;
            while (timePassed < time)
            {
                timePassed += Time.deltaTime;
                yield return null;
            }
            Debug.Log("Stopping the rumble");
            pad.SetMotorSpeeds(0f, 0f);
        }
        public void TestRumble()
        {
            RumblePulse(0.2f, 0.5f, 1f);
        }
    }
}
