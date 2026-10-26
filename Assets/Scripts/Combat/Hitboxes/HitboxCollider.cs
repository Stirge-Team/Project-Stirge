using System.Collections.Generic;
using Stirge.Camera;
using Stirge.Sound;
using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    [RequireComponent(typeof(Collider))]
    public class HitboxCollider : Hitbox
    {
        private Vector3 m_lastPos;
        private Vector3 m_velocity;

        private readonly List<Collider> m_savedColliders = new();
        private readonly Dictionary<Hittable, List<int>> m_appliedStatuses = new();

        private void OnTriggerStay(Collider other)
        {
            //if the checked object's layer is NOT in the layer mask, do nothing
            if (!(((1 << other.gameObject.layer) & m_data.CollisionMask) != 0))
                return;

            //prevent repeat collisions
            if (m_savedColliders.Contains(other))
                return;

            // do OnHit Shtuff
            if (other.TryGetComponent(out Hittable target))
            {
                //add collider to list of collided objects
                m_savedColliders.Add(other);
                OnHittableHit(target);
            }
        }

        private void Update()
        {
            m_velocity = transform.position - m_lastPos;

            m_lastPos = transform.position;
        }

        public override void InitHitbox(SerializedHitboxData hitboxData)
        {
            m_data = hitboxData;
        }
        protected override void EnableHitbox()
        {
            gameObject.SetActive(true);

            //reset collided objects
            m_savedColliders.Clear();
            m_appliedStatuses.Clear();

            //reset vectors
            m_lastPos = transform.position;
            m_velocity = Vector3.zero;
        }
        protected override void DisableHitbox()
        {
            gameObject.SetActive(false);
        }

        private void OnHittableHit(Hittable hitTarget)
        {
            List<int> statusesToInflictIndices = new();
            for (int i = 0, count = m_data.StatusInflictDatas.Length; i < count; i++)
            {
                // if the Status at this index has already been inflicted on this Hittable and should only be inflicted once, skip
                // We use the index of the StatusInflictData from the HitboxData because multiple of those Statuses may inflict the same Status but in different ways
                if (m_appliedStatuses.ContainsKey(hitTarget) && m_appliedStatuses[hitTarget].Contains(i))
                    continue;

                StatusInflictData inflictData = m_data.StatusInflictDatas[i];

                // if the Status at this index should only be inflicted once, add its index to the tracking list
                if (inflictData.InflictOnlyOnce)
                {
                    // create Dictionary entry for this Hittable if it doesn't exist
                    m_appliedStatuses.TryAdd(hitTarget, new());
                    m_appliedStatuses[hitTarget].Add(i);
                }

                // add the StatusActionIndex to the list of Statuses to apply
                statusesToInflictIndices.Add(inflictData.StatusActionIndex);
            }

            if (m_data.CameraShakePreset != null) CameraShakeController.Instance.BeginScreenshake(m_data.CameraShakePreset);
            TimeManager.Instance.SetTimeScaleForTime(m_data.HitstopScale, m_data.HitstopDuration);
            //On hit sound? I'll save this for another branch.
            if(m_data.HitSound != null) SoundManager.Instance.PlaySoundClipAtPosition(m_data.HitSound, m_owner.transform.position);
            hitTarget.OnHit(m_owner, m_data, hitTarget.transform.position, m_velocity, statusesToInflictIndices.ToArray());
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.color = Color.red;
                if (m_velocity != Vector3.zero)
                {
                    Gizmos.DrawLine(Vector3.zero, m_velocity);
                    Gizmos.DrawSphere(m_velocity, .05f);
                }

                Gizmos.color = Color.paleGreen;
                Collider[] cols = GetColliderComponentsInChildrenThatAreNotHitboxCollider();
                foreach (Collider col in cols)
                {
                    if (col is SphereCollider sphere)
                    {
                        Gizmos.DrawWireSphere(sphere.center, sphere.radius);
                    }
                    else if (col is BoxCollider box)
                    {
                        Gizmos.DrawWireCube(box.center, box.size);
                    }
                    else if (col is CapsuleCollider capsule)
                    {
                        Vector3 pos = capsule.center;
                        Vector3 up = pos + new Vector3(0f, capsule.height / 2f - capsule.radius);
                        Vector3 down = pos + new Vector3(0f, -capsule.height / 2f + capsule.radius);
                        float radius = capsule.radius;

                        Gizmos.DrawWireSphere(up, radius);
                        Gizmos.DrawLine(up + Vector3.right * radius, down + Vector3.right * radius);
                        Gizmos.DrawLine(up - Vector3.right * radius, down - Vector3.right * radius);
                        Gizmos.DrawLine(up + Vector3.forward * radius, down + Vector3.forward * radius);
                        Gizmos.DrawLine(up - Vector3.forward * radius, down - Vector3.forward * radius);
                        Gizmos.DrawWireSphere(down, radius);
                    }
                }

                Gizmos.matrix = Matrix4x4.identity;
            }
        }

        private Collider[] GetColliderComponentsInChildrenThatAreNotHitboxCollider()
        {
            List<Collider> foundColliders = new();
            if (TryGetComponent(out Collider col))
            {
                foundColliders.Add(col);
            }
            CheckChildren(transform);
            return foundColliders.ToArray();

            void CheckChildren(Transform root)
            {
                for (int i = 0, count = root.childCount; i < count; i++)
                {
                    Transform child = root.GetChild(i);

                    // if child is a HitboxCollider, allow it to draw its own Hitboxes
                    if (!child.TryGetComponent(out HitboxCollider collider))
                    {
                        if (child.TryGetComponent(out Collider found))
                        {
                            foundColliders.Add(found);
                        }

                        // Check this Transform's children too
                        CheckChildren(child);
                    }
                }
            }
        }
#endif
    }
}

