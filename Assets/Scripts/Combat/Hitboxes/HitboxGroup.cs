using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    public class HitboxGroup : Hitbox
    {
        [SerializeField] private HitboxCollider[] m_hitboxes = new HitboxCollider[0];

        private void Awake()
        {
            if (m_owner == null)
            {
                Debug.LogError("Please provide a reference to the Combat Entity owner of this Hitbox Group!", this);
                enabled = false;
                return;
            }
            foreach (HitboxCollider hitbox in m_hitboxes)
            {
                hitbox.SetOwner(m_owner);
            }
        }

        public override void InitHitbox(SerializedHitboxData hitboxData)
        {
            foreach (HitboxCollider hitbox in m_hitboxes)
            {
                hitbox.InitHitbox(hitboxData);
            }
        }
        protected override void EnableHitbox()
        {
            foreach (HitboxCollider hitbox in m_hitboxes)
            {
                hitbox.isActive = true;
            }
        }
        protected override void DisableHitbox()
        {
            foreach (HitboxCollider hitbox in m_hitboxes)
            {
                hitbox.isActive = false;
            }
        }
    }
}
