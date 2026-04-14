using UnityEngine;
using UnityEngine.Events;
namespace Stirge.Player
{
    public class EntityHealth : MonoBehaviour
    {
        [SerializeField]
        private float m_maxHealth = 10;
        public float _currentHealth {get; private set;}
        public bool _isDead {get; private set;}
        public UnityEvent<float> _healEvents;
        public UnityEvent<float> _modifyEvents;
        public UnityEvent<float> _damageEvents;
        public UnityEvent _deathEvents;
        public float _healthPercent => _currentHealth / m_maxHealth;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Set the health
            _currentHealth = m_maxHealth;
        }
        //TESTFUNCTION - BUTTONS ARE DUMB
        public void UpdateHealth(float amount)
        {
            UpdateHealth(amount, null);
        }
        public void UpdateHealth(float amount, Object sender = null)
        {
            _currentHealth += amount;
            Debug.Log($"Player's health changed by {amount}{(sender ? $" by {sender}.":".")} Now {_currentHealth}/{m_maxHealth}.");
            CheckHealth();
            
            //call events
            if(amount != 0) _modifyEvents.Invoke(amount);
            if(amount > 0) _healEvents.Invoke(amount);
            if(amount < 0) _damageEvents.Invoke(amount);
        }
        
        public void CheckHealth()
        {
            if(_currentHealth <= 0)
            {
                Debug.Log("The player has died!");
                _isDead = true;
                //this can be replaced with other code if needed.
                _deathEvents.Invoke();
            }
        }

    }
}