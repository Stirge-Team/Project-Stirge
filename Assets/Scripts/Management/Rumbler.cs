using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stirge.Management
{
    public class Rumbler : MonoBehaviour
    {
        public static Rumbler Instance => m_instance;
        private static Rumbler m_instance;

        void Awake()
        {
            if (m_instance == null) //if null, attempt to become the new instance
            {
                m_instance = this;
            }
            else if (m_instance.name != name)
            {
                Debug.LogWarning("There are multiple rumblers in scene. There should only be one rumbler, please remove any other instances!");
            }
        }

        public void CallRumble(Vector2 strength, float time)
        {
            StartCoroutine(DoRumble(strength, time));
        }
        private IEnumerator DoRumble(Vector2 strength, float time)
        {
            Gamepad.current.SetMotorSpeeds(strength.x, strength.y);
            InputSystem.ResetHaptics();
            yield return new WaitForSecondsRealtime(time);
        }
    }
}
