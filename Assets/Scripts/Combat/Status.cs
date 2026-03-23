using UnityEngine;

namespace Stirge.Combat
{
    using Enemy;

    [System.Serializable]
    public abstract class Status
    {
        public abstract void Inflict(Enemy enemy);
    }

    [System.Serializable]
    public class Stun : Status
    {
        [SerializeField, Min(0)] private float m_stunLength;

        public override void Inflict(Enemy enemy)
        {
            enemy.EnterStun(m_stunLength);
        }
    }

    [System.Serializable]
    public class Knockback : Status
    {
        [SerializeField] private float m_strength;

        public override void Inflict(Enemy enemy)
        {
            Vector2 dir = new Vector2(enemy.transform.forward.x, enemy.transform.forward.z);
            enemy.EnterKnockback(m_strength, -dir);
        }
    }

    [System.Serializable]
    public class AirJuggle : Status
    {
        [SerializeField] private float m_strength;
        [SerializeField] private float m_airStallLength;

        public override void Inflict(Enemy enemy)
        {
            Vector2 dir = new Vector2(enemy.transform.forward.x, enemy.transform.forward.z);
            enemy.EnterAirJuggle(m_strength, -dir, m_airStallLength);
        }
    }
}
