using Stirge.AttackTimeline;
using UnityEngine;

namespace Stirge.Combat
{
    [RequireComponent(typeof(Collider))]
    public abstract class Hittable : MonoBehaviour
    {
        /// <summary>
        /// Called when a Hittable object is hit via a hitbox collider
        /// </summary>
        /// <param name="hitboxData">parsed hitbox data</param>
        /// <param name="attackingEntity">combat entity that hit the object</param>
        public virtual void OnHit(HitboxData hitboxData, CombatEntity attackingEntity)
        {
            throw new System.NotImplementedException();
        }
    }
}

