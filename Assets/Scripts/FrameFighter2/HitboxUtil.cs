using UnityEngine;

namespace FrameFighter2.Hitbox{
    public static class HitboxUtil
    {
        static readonly Collider[] buffer = new Collider[128];

        public static int OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion rotation, int mask)
        {
            //mask does nothing for now
            return Physics.OverlapBoxNonAlloc(center, halfExtents, buffer, rotation);
        }

        public static int OverlapSphere(Vector3 center, float radius, int mask)
        {
            //mask does nothing for now
            return Physics.OverlapSphereNonAlloc(center, radius, buffer);
        }

        public static int OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int mask)
        {
            //mask does nothing for now
            return Physics.OverlapCapsuleNonAlloc(point0, point1, radius, buffer);
        }

        public static Collider Get(int index)
        {
            return buffer[index];
        }
    }

}
