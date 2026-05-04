using System.Collections;
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

        [System.Flags]
        public enum InvincibilityType
        {
            None = 0,
            NoDamageHealth = 1,
            NoDamageMaxHealth = 2,
            NoDamage = 3, //1, 2
            NoModifiationsHealth = 4,
            NoModifiationsMaxHealth = 8,
            NoModifiations = 12, //4, 8
            Immortality = 16
        };
        private InvincibilityType m_invincibility;

        public void Start()
        {
            _maxHealth = _currentHealth = m_baseHealth;
        }
        #region Health
        public void ModifyHealth(float amount, bool clamp = true, Object sender = null)
        {
            if ((amount < 0 && m_invincibility.HasFlag(InvincibilityType.NoDamageHealth)) || m_invincibility.HasFlag(InvincibilityType.NoModifiationsHealth))
                amount = 0;

            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, clamp ? _maxHealth : Mathf.Infinity);

            Debug.Log($"{name}'s health changed by {amount}{(sender ? $" by {sender}." : ".")} Now {_currentHealth}/{_maxHealth}.");
            CheckHealth();
        }
        public void ModifyHealth(bool attemptRevive, float amount, bool clamp = true, Object sender = null)
        {
            if (attemptRevive)
                Revive();
            ModifyHealth(amount, clamp, sender);
        }
        public void CheckHealth()
        {
            if (_healthPercent <= 0)
                if (m_invincibility.HasFlag(InvincibilityType.Immortality))
                {
                    if(_currentHealth <= 0) _currentHealth = 1;
                    if(_maxHealth <= 0) _maxHealth = 1;
                    Debug.Log($"But {name} is immortal!");
                }
                else
                {
                    Debug.Log($"{name} has died!");
                    _isDead = true;
                    StopAllCoroutines();
                    m_invincibility = 0;
                }
        }
        #endregion
        public void Revive(bool reset = false)
        {
            if (_isDead)
            {
                _isDead = false;
                if (reset)
                {
                    _maxHealth = _currentHealth = m_baseHealth;
                }
                Debug.Log($"{name} has been revived!");
            }
            else Debug.LogWarning($"{name} cannot be revived as they aren't dead.");
        }
        #region Max Health
        public void ModifiyMaximumHealth(float amount, MaxHealthChangeBehaviour behaviour, Object sender = null)
        {
            if ((amount < 0 && m_invincibility.HasFlag(InvincibilityType.NoDamageMaxHealth)) || m_invincibility.HasFlag(InvincibilityType.NoModifiationsMaxHealth))
                amount = 0;

            //either clamp reguardless or clamp if the entity's health is equal or less than the current maximum.
            bool doClamp = behaviour.HasFlag(MaxHealthChangeBehaviour.ClampToNewMax) || (_currentHealth <= _maxHealth && behaviour.HasFlag(MaxHealthChangeBehaviour.ClampOnlyIfNoExtraHealth));
            //prepare to find the delta
            float maxHealthDelta = _maxHealth;
            //apply change to the maximum health
            _maxHealth = Mathf.Clamp(_maxHealth + amount, 0, Mathf.Infinity);
            Debug.Log($"{name}'s maximum health changed by {amount}, now {_maxHealth}.");
            //find the delta
            maxHealthDelta = _maxHealth / maxHealthDelta;

            //apply the same change to the current health (if intended)
            Debug.Log($"Applying {behaviour} behaviour...");
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
        #endregion
        #region Invincibility
        public void StartInvincibility(float time, InvincibilityType type)
        {
            Debug.Log($"{name} is now {type}");
            m_invincibility |= type;
            if (time > 0) StartCoroutine(InvincibilityCountdown(time, type));
        }
        public void EndInvincibility(InvincibilityType type)
        {
            Debug.Log($"{name} is no longer {type}");
            m_invincibility &= ~type;
        }
        private IEnumerator InvincibilityCountdown(float time, InvincibilityType type)
        {
            yield return new WaitForSeconds(time);
            EndInvincibility(type);
        }

        //Button/Unity Event calls
        public void StartInvincibility(float time)
        {
            StartInvincibility(time, InvincibilityType.NoDamage);
        }
        public void StartUnmodifiable(float time)
        {
            StartInvincibility(time, InvincibilityType.NoModifiations);
        }
        public void StartImmortality(float time)
        {
            StartInvincibility(time, InvincibilityType.Immortality);
        }
        public void EndInvincibility(int type)
        {
            EndInvincibility((InvincibilityType)type);
        }
        #endregion
    }
}