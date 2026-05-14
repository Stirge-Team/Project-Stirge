using UnityEngine;

namespace Stirge.Combat
{
    [System.Serializable]
    public abstract class Status
    {
        protected bool m_isCleared = false;
        public bool IsCleared => m_isCleared;

        public virtual void OnInflict(CombatEntity entity) { }
     
        public static readonly System.Type[] StatusTypes =
        {
            typeof(AirJuggle),
            typeof(Knockback),
            typeof(Stun)
        };
    }

    public abstract class TimedStatus : Status
    {
        public TimedStatus(float length)
        {
            m_length = length;
        }
        public TimedStatus(TimedStatus original)
        {
            m_length = original.m_length;
        }
        
        [SerializeField] private float m_length;

        private float m_timer;

        public override void OnInflict(CombatEntity entity)
        {
            m_timer = m_length;
        }

        public virtual void Update(CombatEntity entity, float deltaTime)
        {
            m_timer -= deltaTime;

            if (m_timer <= 0)
            {
                m_isCleared = true;
            }
        }

        public virtual void OnClear(CombatEntity entity) { }
    }

    [System.Serializable]
    public class Stun : TimedStatus
    {
        public Stun() : base(1f) { }
        public Stun(float length) : base(length) { }
        public Stun(Stun original) : base(original) { }

        public override void OnInflict(CombatEntity entity)
        {
            base.OnInflict(entity);
            entity.SetIsStunned(true);
        }

        public override void OnClear(CombatEntity entity)
        {
            entity.SetIsStunned(false);
        }
    }

    [System.Serializable]
    public class Knockback : Status
    {
        public Knockback()
        {
            m_strength = 1f;
            m_height = 1f;
        }
        public Knockback(float strength, float height)
        {
            m_strength = strength;
            m_height = height;
        }

        [SerializeField, Min(0f)] private float m_strength;
        [SerializeField, Min(0f)] private float m_height;

        public override void OnInflict(CombatEntity entity)
        {
            Vector3 dir = -entity.transform.GetChild(0).forward;
            entity.EnterKnockback(m_strength, dir, m_height, 0);
        }
    }

    [System.Serializable]
    public class AirJuggle : Status
    {
        public AirJuggle()
        {
            m_strength = 1f;
            m_stallLength = 1f;
        }
        public AirJuggle(float strength, float stallLength)
        {
            m_strength = strength;
            m_stallLength = stallLength;
        }
        
        [SerializeField, Min(0f)] private float m_strength;
        [SerializeField, Min(0f)] private float m_stallLength;

        public override void OnInflict(CombatEntity entity)
        {
            entity.EnterAirJuggle(m_strength, Vector3.up, m_stallLength, 0);
        }
    }
}
