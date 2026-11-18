using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    using UtilityAI;

    public abstract class Hittable : MonoBehaviour
    {
        /// <summary>
        /// Add to this Event to perform logic when this <see cref="Hittable"/> is hit.<br/>
        /// Param 0 'attacker': The <see cref="CombatEntity"/> that hit this <see cref="Hittable"/>.<br/>
        /// Param 1 'hitPosition': The world position of the hit.<br/>
        /// Param 2 'hitVelocity': The velocity of what hit this.
        /// </summary>
        public event System.Action<CombatEntity, Vector3, Vector3> OnHit;

        /// <summary>
        /// Called when a <see cref="Hittable"/> object is hit by a <see cref="HitboxCollider"/>.
        /// </summary>
        /// <param name="attacker"><see cref="CombatEntity"/> that hit the <see cref="Hittable"/>.</param>
        /// <param name="statusData">Data about the Status.</param>
        /// <param name="hitPosition">World position of the hit.</param>
        /// <param name="hitVelocity">Velocity of what hit this.</param>
        /// <param name="statusesToInflict">Statuses that will be inflicted to a hit CombatEntity.</param>
        public void Hit(CombatEntity attacker, SerializedHitboxData statusData, Vector3 hitPosition, Vector3 hitVelocity, Status[] statusesToInflict = null)
        {
            HitInternal(attacker, statusData, hitPosition, hitVelocity, statusesToInflict);
            OnHit?.Invoke(attacker, hitPosition, hitVelocity);
        }

        /// <summary>
        /// Override to write logic for how this <see cref="Hittable"/> should handle getting Hit.
        /// </summary>
        protected abstract void HitInternal(CombatEntity attacker, SerializedHitboxData statusData, Vector3 hitPosition, Vector3 hitVelocity, Status[] statusesToInflict = null);
    }
}

