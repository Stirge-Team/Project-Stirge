using UnityEngine;

namespace Stirge.Management
{
    public class EntityHealth : MonoBehaviour
    {
        [System.Flags]
        public enum MaxHealthChangeBehaviour
        {
            MatchReductions = 1,
            MatchIncreases = 2,
            MatchAny = 3,
            EquivalentReduction = 4,
            EquivalentIncrease = 8,
            EquivalentAny = 12,
            ClampToNewMax = 16,
            ClampOnlyIfNoExtraHealth = 32,
            //Set the current health to the new maximum
            Set = 64
        };
        [SerializeField, Tooltip("The initial health of this entity.")]
        private float m_baseHealth;
        public float _maxHealth { get; private set; }
        public float _currentHealth { get; private set; }
        public float _healthPercent => _currentHealth / _maxHealth;
        public bool _isDead { get; private set; }

        public void Start()
        {
            _maxHealth = _currentHealth = m_baseHealth;
        }
        public void ModifyHealth(float amount, bool clamp = true, Object sender = null)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, clamp ? _maxHealth : Mathf.Infinity);
            Debug.Log($"This entity's health changed by {amount}{(sender ? $" by {sender}." : ".")} Now {_currentHealth}/{_maxHealth}.");
            CheckHealth();
        }
        public void ModifyHealth(bool attemptRevive, float amount, bool clamp = true, Object sender = null)
        {
            if(attemptRevive)
                Revive();
            ModifyHealth(amount, clamp, sender);
        }
        public void CheckHealth()
        {
            if (_currentHealth <= 0)
            {
                Debug.Log("This entity has died!");
                _isDead = true;
            }
        }
        public void Revive(bool reset = false)
        {
            if (_isDead)
            {
                _isDead = false;
                if(reset)
                {
                    _maxHealth = _currentHealth = m_baseHealth;
                }
                Debug.Log($"A entity has been revived!");
            }
            else Debug.LogWarning($"The entity you are to revive isn't dead.");
        }
        public void ModifiyMaximumHealth(float amount, MaxHealthChangeBehaviour behaviour, Object sender = null)
        {
            //either clamp reguardless or clamp if the entity's health is equal or less than the current maximum.
            bool doClamp = behaviour.HasFlag(MaxHealthChangeBehaviour.ClampToNewMax) || (_currentHealth <= _maxHealth && behaviour.HasFlag(MaxHealthChangeBehaviour.ClampOnlyIfNoExtraHealth));
            //prepare to find the delta
            float maxHealthDelta = _maxHealth;
            //apply change to the maximum health
            _maxHealth = Mathf.Clamp(_maxHealth + amount, 0, Mathf.Infinity);
            //find the delta
            maxHealthDelta = _maxHealth / maxHealthDelta;

            //apply the same change to the current health (if intended)
            if ((behaviour.HasFlag(MaxHealthChangeBehaviour.MatchReductions) && amount < 0) || (behaviour.HasFlag(MaxHealthChangeBehaviour.MatchIncreases) && amount > 0))
            {
                ModifyHealth(amount, doClamp, sender);
            }
            //otherwise, apply the delta of the max health to the current (if intended)
            else if ((behaviour.HasFlag(MaxHealthChangeBehaviour.EquivalentReduction) && amount < 0) || (behaviour.HasFlag(MaxHealthChangeBehaviour.EquivalentIncrease) && amount > 0))
            {
                ModifyHealth(-_currentHealth * (1 - maxHealthDelta), doClamp, sender);
            }
            else if (behaviour.HasFlag(MaxHealthChangeBehaviour.Set))
            {
                ModifyHealth(_maxHealth - _currentHealth, doClamp, sender);
            }
            else //otherwise, just clamp/check the current health
            {
                ModifyHealth(0, doClamp, sender);
            }
        }
    }
}