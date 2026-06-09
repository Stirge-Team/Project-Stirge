using UnityEngine;

namespace Stirge.Combat
{
    [System.Serializable]
    public abstract class Status
    {
        protected bool m_isCleared = false;
        public bool IsCleared => m_isCleared;

        public virtual void OnInflict(CombatEntity targetEntity)
        {
            throw new System.NotImplementedException();
        }
        public virtual void OnInflict(CombatEntity targetEntity, CombatEntity attackingEntity)
        {
            OnInflict(targetEntity);
        }

        public static readonly System.Type[] StatusTypes =
        {
            typeof(AirJuggle),
            typeof(Knockback),
            typeof(Stun)
        };
    }

    [System.Serializable]
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

        protected float Length => m_length;

        public override void OnInflict(CombatEntity targetEntity)
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

        public override void OnInflict(CombatEntity targetEntity)
        {
            base.OnInflict(targetEntity);
            targetEntity.SetIsStunned(true, Length);
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

        public override void OnInflict(CombatEntity targetEntity, CombatEntity attackingEntity)
        {
            Vector3 dir = attackingEntity.GetForward();
            targetEntity.EnterKnockback(m_strength, dir, m_height, 0);
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

        public override void OnInflict(CombatEntity targetEntity)
        {
            targetEntity.EnterAirJuggle(m_strength, Vector3.up, m_stallLength, 0);
        }
    }
}
