using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    [System.Serializable]
    public class SerializedHitboxData
    {
        [Tooltip("The Mask the HitboxCollider will use to check for Hittables.")]
        [SerializeField] private LayerMask m_collisionMask;
        [Tooltip("The damage that will be dealt to the Hittable when hit by the HitboxCollider.")]
        [SerializeField] private int m_damage;
        [Tooltip("Details for the Statuses to inflict.")]
        [SerializeField] private StatusInflictData[] m_inflictedStatuses;

        public LayerMask CollisionMask => m_collisionMask;
        public int Damage => m_damage;
        public StatusInflictData[] InflictedStatuses => m_inflictedStatuses;
    }
}
