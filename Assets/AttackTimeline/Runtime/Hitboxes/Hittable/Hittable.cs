using UnityEngine;

namespace Stirge.Combat
{
    using AttackTimeline;

    [RequireComponent(typeof(Collider))]
    public abstract class Hittable : MonoBehaviour
    {
        /// <summary>
        /// Called when a <see cref="Hittable"/> object is hit by a <see cref="HitboxCollider"/>.
        /// </summary>
        /// <param name="hitboxData">parsed hitbox data</param>
        /// <param name="attackingEntity">combat entity that hit the object</param>
        public virtual void OnHit(HitboxData hitboxData, CombatEntity attackingEntity)
        {
            throw new System.NotImplementedException();
        }
    }
}

