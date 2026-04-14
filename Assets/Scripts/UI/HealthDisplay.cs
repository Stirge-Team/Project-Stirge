using System;
using System.Collections;
using Stirge.Management;
using UnityEngine;
using UnityEngine.UI;

namespace Stirge.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        [Serializable]
        public class DynamicHealthBar
        {
            [SerializeField, Tooltip("The image object used to display info.")]
            public Image _display;
            [SerializeField, Tooltip("How long to wait after health has increased to begin moving.")]
            public float _healHoldTime;
            [SerializeField, Tooltip("How quickly the health bar will increase.")]
            public float _growthRate;
            [SerializeField, Tooltip("How long to wait after health has decreased to begin moving.")]
            public float _damageHoldTime;
            [SerializeField, Tooltip("How quickly the health bar will decrease.")]
            public float _decayRate;
            public bool _hideWhenFinished = false;
            public IEnumerator UpdateDisplay(float amount)
            {
                _display.enabled = true;

                //if the player was damaged
                if(_display.fillAmount > amount && _decayRate > 0)
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
                else if (_display.fillAmount < amount && _growthRate > 0)
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

        [SerializeField, Tooltip("The entity who health you like to display.")]
        private EntityHealth m_healthToDisplay;
        [SerializeField]
        private DynamicHealthBar m_healthBar;
        private IEnumerator m_healthBarUpdate => m_healthBar.UpdateDisplay(m_healthToDisplay._health._healthPercent);
        [SerializeField]
        private DynamicHealthBar m_damageBar;
        private IEnumerator m_damageBarUpdate => m_damageBar.UpdateDisplay(m_healthToDisplay._health._healthPercent);
        [SerializeField]
        private DynamicHealthBar m_healBar;
        private IEnumerator m_healBarUpdate => m_healBar.UpdateDisplay(m_healthToDisplay._health._healthPercent);
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            m_healthBar._display.fillAmount = 1; //m_healthToDisplay._PLEASENAMEME._healthPercent;
            m_damageBar._display.fillAmount = 1; //m_healthToDisplay._PLEASENAMEME._healthPercent;
            m_healBar._display.fillAmount = 1; //m_healthToDisplay._PLEASENAMEME._healthPercent;
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