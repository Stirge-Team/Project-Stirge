using Stirge.Combat;
using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    public abstract class Hitbox : MonoBehaviour
    {
        [SerializeField] protected CombatEntity m_owner;

        protected SerializedHitboxData m_data;

        private bool m_isActive;
        public bool isActive
        {
            get => m_isActive;
            set
            {
                if (value != m_isActive)
                {
                    m_isActive = value;
                    if (value)
                        EnableHitbox();
                    else
                        DisableHitbox();
                }
            }
        }

        public CombatEntity Owner => m_owner;

        public abstract void InitHitbox(SerializedHitboxData hitboxData);
        public abstract void EnableHitbox();
        public abstract void DisableHitbox();
    }
}
