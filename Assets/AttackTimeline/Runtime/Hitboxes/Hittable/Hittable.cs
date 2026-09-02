using Stirge.AttackTimeline;
using UnityEngine;

namespace Stirge.Combat
{
    [RequireComponent(typeof(Collider))]
    public abstract class Hittable : MonoBehaviour
    {
        public virtual void OnHit(HitboxData hitboxData, object parsedOwner = null)
        {

        }
    }
}

