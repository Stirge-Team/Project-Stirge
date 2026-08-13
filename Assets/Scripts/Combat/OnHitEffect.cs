using UnityEngine;

namespace Stirge.Combat
{
    using Enemy;
    using OldStatus;

    [System.Serializable]
    public class OnHitEffect
    {
        [SerializeField] private int m_damage;
        [SerializeReference] private Status[] m_statuses = new Status[0];

        public void OnHit(CombatEntity targetEntity, CombatEntity attackingEntity)
        {
            targetEntity.TakeDamage(m_damage);
            if (!targetEntity.Health._isDead)
            {
                foreach (Status status in m_statuses)
                {
                    if (status is TimedStatus)
                    {
                        //targetEntity.InflictTimedStatus(status as TimedStatus, attackingEntity);
                        return;
                    }
                    else
                    {
                        //targetEntity.InflictStatus(status, attackingEntity);
                        return;
                    }
                }
            }
        }
    }
}