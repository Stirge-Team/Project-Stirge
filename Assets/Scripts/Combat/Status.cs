using UnityEngine;

namespace Stirge.Combat
{
    [System.Serializable]
    public abstract class Status
    {
        public static readonly System.Type[] StatusTypes =
        {
            typeof(AirJuggle),
            typeof(Knockback),
            typeof(Stun)
        };

        private bool m_isCleared = false;
        public bool IsCleared => m_isCleared;

        public abstract void Inflict(CombatEntity entity);
        public virtual void Update(CombatEntity entity) { }
    }

    [System.Serializable]
    public class Stun : Status
    {
        [SerializeField, Min(0)] private float m_stunLength;

        public override void Inflict(CombatEntity entity)
        {
            entity.EnterStun(m_stunLength);
        }
    }

    [System.Serializable]
    public class Knockback : Status
    {
        [SerializeField, Min(0f)] private float m_strength;
        [SerializeField, Min(0f)] private float m_height;
        [SerializeField, Min(0f)] private float m_stunLength;

        public override void Inflict(CombatEntity entity)
        {
            Vector3 dir = -entity.transform.GetChild(0).forward;
            entity.EnterKnockback(m_strength, dir, 1f, m_stunLength);
        }
    }

    [System.Serializable]
    public class AirJuggle : Status
    {
        [SerializeField] private float m_strength;
        [SerializeField] private float m_airStallLength;
        [SerializeField, Min(0f)] private float m_stunLength;

        public override void Inflict(CombatEntity entity)
        {
            entity.EnterAirJuggle(m_strength, Vector3.up, m_airStallLength, m_stunLength);
        }
    }
}
