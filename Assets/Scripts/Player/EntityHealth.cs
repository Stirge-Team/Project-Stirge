using UnityEngine;
using UnityEngine.Events;

namespace Stirge.Management
{
    public class EntityHealth : MonoBehaviour
    {
        [System.Serializable]
        public class Health
        {
            [SerializeField, Tooltip("The initial health of this entity.")]
            private float m_baseHealth;
            public float _maxHealth { get; private set; }
            public float _currentHealth { get; private set; }
            public float _healthPercent => _currentHealth / m_baseHealth;
            public bool _isDead { get; private set; }

            public void Initialise()
            {
                _maxHealth = _currentHealth = m_baseHealth;
            }
            public void UpdateHealth(float amount, bool clamp = true, Object sender = null)
            {
                _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, clamp ? _maxHealth : Mathf.Infinity);
                Debug.Log($"Player's health changed by {amount}{(sender ? $" by {sender}." : ".")} Now {_currentHealth}/{m_baseHealth}.");
                CheckHealth();
            }
            public void CheckHealth()
            {
                if (_currentHealth <= 0)
                {
                    Debug.Log("The player has died!");
                    _isDead = true;
                }
            }
            public void Revive()
            {
                if(_isDead)
                {
                    _isDead = false;
                    Debug.Log($"A entity has been revived!");
                }
                else Debug.LogWarning($"The entity you are to revive isn't dead.");
            }
        }
        [SerializeField]
        private Health m_health;
        public Health _getHealth => m_health;
        //EVENTS
        [SerializeField, Tooltip("Events to call only when this entity's health increases.")]
        private UnityEvent<float> _healEvents;
        [SerializeField, Tooltip("Events to call when this entity's health is changed")]
        private UnityEvent<float> _modifyEvents;
        [SerializeField, Tooltip("Events to call only when this entity takes damage")]
        private UnityEvent<float> _damageEvents;
        [SerializeField, Tooltip("Events to call when this entity's health reaches 0")]
        private UnityEvent _deathEvents;
        [SerializeField, Tooltip("Events to do alongside the health modification. Allows other scripts to access the health values.")]
        private UnityEvent<Health, float, bool, Object> _extraEvents;
        [SerializeField, Tooltip("Enable this if you want the base health modification function to be overriden.")]
        private bool m_overrideBaseFunction;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Set the health
            m_health.Initialise();
        }
        //TESTFUNCTION - BUTTONS ARE DUMB
        public void ButtonUpdateHealth(float amount)
        {
            UpdateHealth(amount);
        }
        public void UpdateHealth(float amount, bool clamp = true, Object sender = null)
        {
            if(m_health._isDead) return;

            //check if there are any overriding events.
            if (!m_overrideBaseFunction)
            {
                m_health.UpdateHealth(amount, clamp, sender);
            }
            _extraEvents.Invoke(m_health, amount, clamp, sender);

            //call events
            if (m_health._isDead) { _deathEvents.Invoke();}
            if (amount != 0) _modifyEvents.Invoke(amount);
            if (amount > 0) _healEvents.Invoke(amount);
            if (amount < 0) _damageEvents.Invoke(amount);
        }
        public void ReviveEntity()
        {
            m_health.Revive();
            UpdateHealth(m_health._maxHealth);
        }
    }
}