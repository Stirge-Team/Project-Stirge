using System;
using Stirge.Camera;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Stirge.TestScript
{
    public class CameraShakeSceneControls : MonoBehaviour
    {
        public TMP_InputField m_ampField;
        public TMP_InputField m_freqField;
        public TMP_InputField m_rateField;

        public void DoTheThing()
        {
            FindAnyObjectByType<CameraShakeController>().BeginScreenshake(new((float)Convert.ToDouble(m_ampField.text), (float)Convert.ToDouble(m_freqField.text), (float)Convert.ToDouble(m_rateField.text)));
        }
    }
}
