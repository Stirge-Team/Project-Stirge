using UnityEngine;

namespace Stirge.Combat
{
    using Enemy;

    [System.Serializable]
    public class OnHitEffect
    {
        [SerializeField] private int m_damage;
        [SerializeReference] private Status[] m_statuses = new Status[0];

        public void OnHit(CombatEntity entity)
        {
            entity.TakeDamage(m_damage);
            if (!entity.Health._isDead)
            {
                foreach (Status status in m_statuses)
                {
                    status.OnInflict(entity);
                }
            }
        }
    }
}