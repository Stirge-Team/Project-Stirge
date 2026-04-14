using System;
using System.Collections;
using Stirge.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Stirge.UI
{

    public class HealthDisplay : MonoBehaviour
    {
        [Serializable]
        public class DynamicBar
        {
            public Image _display;
            public float _healHoldTime;
            public float _growthRate;
            public float _damageHoldTime;
            public float _decayRate;
            public bool _hideWhenFinished = false;
            public IEnumerator UpdateDisplay(float amount, bool valueIsPercent = true)
            {
                _display.enabled = true;
                //if the player was damaged
                if(_display.fillAmount > amount && valueIsPercent && _decayRate > 0)
                {
                    yield return new WaitForSeconds(_damageHoldTime);
                    //and the display is greater than the health percent.
                    while(_display.fillAmount > amount)
                    {
                        _display.fillAmount -= _decayRate;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                }
                //else if the player was healed
                else if (_display.fillAmount < amount && valueIsPercent && _growthRate > 0)
                {
                    yield return new WaitForSeconds(_healHoldTime);
                    //and the display is less than the health percent.
                    while(_display.fillAmount < amount)
                    {
                        _display.fillAmount += _growthRate;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }
                }
                _display.fillAmount = amount;
                if(_hideWhenFinished)
                {
                    _display.enabled = false;
                }
            }
        }

        [SerializeField]
        protected EntityHealth m_healthToDisplay;
        [SerializeField]
        private DynamicBar m_healthBar;
        private IEnumerator m_healthBarUpdate => m_healthBar.UpdateDisplay(m_healthToDisplay._healthPercent);
        [SerializeField]
        private DynamicBar m_damageBar;
        private IEnumerator m_damageBarUpdate => m_damageBar.UpdateDisplay(m_healthToDisplay._healthPercent);
        [SerializeField]
        private DynamicBar m_healBar;
        private IEnumerator m_healBarUpdate => m_healBar.UpdateDisplay(m_healthToDisplay._healthPercent);
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            m_healthBar._display.fillAmount = m_healthToDisplay._healthPercent;
            m_damageBar._display.fillAmount = m_healthToDisplay._healthPercent;
            m_healBar._display.fillAmount = m_healthToDisplay._healthPercent;
        }

        public void UpdateHealthDisplay(float amount)
        {
            StopAllCoroutines();
            StartCoroutine(m_healthBarUpdate);
            StartCoroutine(m_damageBarUpdate);
            StartCoroutine(m_healBarUpdate);
        }
    }
}