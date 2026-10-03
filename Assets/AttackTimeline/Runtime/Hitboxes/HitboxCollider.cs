using UnityEngine;
using System.Collections.Generic;

namespace Stirge.AttackTimeline
{
    using Combat;

    [RequireComponent(typeof(Collider))]
    public class HitboxCollider : MonoBehaviour
    {
        [SerializeField] private CombatEntity m_owner;

        private HitboxData m_data = new();

        private Vector3 m_lastPos;
        private Vector3 m_velocityVector;

        public HitboxData Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        private List<Collider> m_savedColliders = new();

        private void OnTriggerStay(Collider other)
        {
            //if the checked object's layer is NOT in the layer mask, do nothing
            if (!(((1 << other.gameObject.layer) & m_data.Mask.value) != 0)) return;

            //prevent repeat collisions
            if (m_savedColliders.Contains(other)) return;
            //add collider to list of collided objects 
            m_savedColliders.Add(other);

            // do OnHit Shtuff
            Hittable hittableObject = other.GetComponent<Hittable>();

            m_data.HitboxWorldPosition = transform.position;
            m_data.HitboxVelocityVector = m_velocityVector;
            if (hittableObject) hittableObject.OnHit(m_data, m_owner);
        }

        private void Update()
        {
            m_velocityVector = (transform.position - m_lastPos).normalized;

            m_lastPos = transform.position;
        }

        private void OnEnable()
        {
            m_lastPos = transform.position;
            m_velocityVector = Vector3.zero;
        }

        public void CreateHitbox(HitboxData data)
        {
            //reset collided objects
            m_savedColliders = new();
            //set hitbox data
            m_data = data;
            //reset vectors
            m_lastPos = transform.position;
            m_velocityVector = Vector3.zero;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + m_velocityVector);
                Gizmos.DrawSphere(transform.position + m_velocityVector, .05f);
            }
        }
#endif
    }
}

