using System;
using FrameFighter2.Hitbox;
using Stirge.Camera;
using Unity.Cinemachine;
using UnityEngine;

namespace Stirge.Combat
{
    [System.Serializable]
    public abstract class Status
    {
        protected bool m_isCleared = false;
        public bool IsCleared => m_isCleared;

        public virtual void OnInflict(Transform hitboxTransform, CombatEntity targetEntity)
        {
            throw new System.NotImplementedException();
        }
        public virtual void OnInflict(Transform hitboxTransform, CombatEntity targetEntity, CombatEntity attackingEntity)
        {
            OnInflict(hitboxTransform, targetEntity);
        }

        public static readonly System.Type[] StatusTypes =
        {
            typeof(AirJuggle),
            typeof(Knockback),
            typeof(Stun),
            typeof(HitStopStatus),
            typeof(ScreenShakeEffect),
            typeof(ParticleEffectOnHit)
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

        public override void OnInflict(Transform hitBoxTransform, CombatEntity targetEntity)
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

        public override void OnInflict(Transform hitboxTransform, CombatEntity targetEntity)
        {
            base.OnInflict(hitboxTransform, targetEntity);
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

        public override void OnInflict(Transform hitboxTransform, CombatEntity targetEntity, CombatEntity attackingEntity)
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

        public override void OnInflict(Transform hitboxTransform, CombatEntity targetEntity)
        {
            targetEntity.EnterAirJuggle(m_strength, Vector3.up, m_stallLength, 0);
        }
    }

    [System.Serializable]
    public class HitStopStatus : Status
    {
        public HitStopStatus()
        {
            m_duration = 1f;
            m_scale = 0f;
        }
        public HitStopStatus(float duration, float scale)
        {
            m_duration = duration;
            m_scale = scale;
        }

        [SerializeField, Min(0f)] private float m_duration;
        [SerializeField, Range(0f, 1f)] private float m_scale;

        public override void OnInflict(Transform hitboxTransform, CombatEntity targetEntity)
        {
            TimeManager.Instance.SetTimeScaleForTime(m_scale, m_duration);//wanna change function
        }
    }

    [System.Serializable]
    public class ScreenShakeEffect : Status
    {
        public ScreenShakeEffect()
        {
            m_preset = null;
        }
        public ScreenShakeEffect(CameraShakePreset preset)
        {
            m_preset = preset;
        }

        [SerializeField] private CameraShakePreset m_preset;

        public override void OnInflict(Transform hitboxTransform, CombatEntity targetEntity)
        {
            CameraShakeController.Instance.BeginScreenshake(m_preset);
        }
    }
}
