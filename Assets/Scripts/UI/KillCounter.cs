using TMPro;
using UnityEngine;

namespace Stirge.Management
{
    public class KillCounter : MonoBehaviour
    {
        private static KillCounter m_inst;
        public static KillCounter Instance => m_inst;
        [SerializeField, Tooltip("How the kill counter should be displayed.\nUse \"?\" as a stand-in for the number.")]
        private string m_textFormat;
        private int m_killCount = -1;
        [SerializeField]
        private TextMeshProUGUI m_textElement;
        void Start()
        {
            if(m_textElement == null)
            {
                Debug.LogError("No text element set, so there is nowhere for the counter to be displayed!", this);
                return;
            }
            if(m_inst == null)
            {
                m_inst = this;
            }
            else if(m_inst != this)
            {
                Debug.LogWarning("There are 2 KillCounters in scene. Please remove one of them");
                enabled = false;
            }
            UpdateCounter();
        }
        public void UpdateCounter()
        {
            m_killCount++;
            m_textElement.text = m_textFormat.Replace("?", m_killCount.ToString());
        }
    }
}
