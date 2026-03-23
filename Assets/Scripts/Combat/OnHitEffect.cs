using UnityEngine;

namespace Stirge.Combat
{
    using Enemy;

    [System.Serializable]
    public class OnHitEffect
    {
        [SerializeField] private int m_damage;
        [SerializeReference] private Status[] m_statuses = new Status[0];

        public void OnHit(Enemy enemy)
        {
            enemy.TakeDamage(m_damage);
            if (!enemy.IsDead())
            {
                foreach (Status status in m_statuses)
                {
                    status.Inflict(enemy);
                }
            }
        }
    }
}