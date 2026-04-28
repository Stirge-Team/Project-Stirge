using UnityEngine;
using UnityEngine.Events;

namespace Stirge.Management
{
    [RequireComponent(typeof(EntityHealth))]
    public class BaseEntityHealthInterp : MonoBehaviour
    {
        private EntityHealth m_health;
        public EntityHealth _getHealth => m_health;
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
        private UnityEvent<EntityHealth, float, bool, Object> _extraEvents;
        [SerializeField, Tooltip("Enable this if you want the base health modification function to be overriden.")]
        private bool m_overrideBaseFunction;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Set the health
            m_health = GetComponent<EntityHealth>();
        }
        //TESTFUNCTION - BUTTONS ARE DUMB
        public void ButtonUpdateHealth(float amount)
        {
            ModifyHealth(amount, false);
        }
        public void ModifyHealth(float amount, bool clamp = true, Object sender = null)
        {
            if (m_health._isDead) return;

            //check if there are any overriding events.
            if (!m_overrideBaseFunction || _extraEvents.GetPersistentEventCount() == 0)
            {
                m_health.ModifyHealth(amount, clamp, sender);
            }
            _extraEvents.Invoke(m_health, amount, clamp, sender);

            InvokeEvents(amount);
        }
        public void ReviveEntity()
        {
            m_health.Revive(true);
            ModifyHealth(1);
        }
        public void ModifiyMaximumHealth(float amount, EntityHealth.MaxHealthChangeBehaviour behaviour, Object sender = null)
        {
            if (m_health._isDead) return;

            m_health.ModifiyMaximumHealth(amount, behaviour, sender);
            InvokeEvents(amount);
        }
        private void InvokeEvents(float amount)
        {
            //call events
            if (m_health._isDead) { _deathEvents.Invoke(); }
            if (amount != 0) _modifyEvents.Invoke(amount);
            if (amount > 0) _healEvents.Invoke(amount);
            if (amount < 0) _damageEvents.Invoke(amount);
        }
        public void ButtonUpdateMax(float amount)
        {
            ModifiyMaximumHealth(amount, EntityHealth.MaxHealthChangeBehaviour.MatchIncreases);
        }
    }
}