using Stirge.Combat;
using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    public class HitboxGroup : Hitbox
    {
        [SerializeField] private HitboxCollider[] m_hitboxes = new HitboxCollider[0];

        public override void InitHitbox(SerializedHitboxData hitboxData)
        {
            foreach (HitboxCollider hitbox in m_hitboxes)
            {
                hitbox.InitHitbox(hitboxData);
            }
        }
        public override void EnableHitbox()
        {
            foreach (HitboxCollider hitbox in m_hitboxes)
            {
                hitbox.EnableHitbox();
            }
        }
        public override void DisableHitbox()
        {
            foreach (HitboxCollider hitbox in m_hitboxes)
            {
                hitbox.DisableHitbox();
            }
        }
    }
}
