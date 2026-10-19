using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    public abstract class Hittable : MonoBehaviour
    {
        /// <summary>
        /// Called when a <see cref="Hittable"/> object is hit by a <see cref="HitboxCollider"/>.
        /// </summary>
        /// <param name="attacker"><see cref="CombatEntity"/> that hit the <see cref="Hittable"/>.</param>
        /// <param name="statusData">Data about the Status.</param>
        /// <param name="statusesToApplyIndices">Indices on their Action of the Statuses to apply that will be applied to a hit CombatEntity.</param>
        public abstract void OnHit(CombatEntity attacker, SerializedHitboxData statusData, Vector3 hitPosition, Vector3 hitVelocity, int[] statusesToApplyIndices);
    }
}

