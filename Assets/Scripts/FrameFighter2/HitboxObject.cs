using FrameFighter2.Manager;
using UnityEngine;
using static FrameFighter2.Data.HitboxData;


namespace FrameFighter2.Hitbox
{
    public class HitboxObject : MonoBehaviour
    {
        private int m_groupID;
        private string m_onHit;
        private int m_endFrame;
        private HitboxShapes m_shape;
        private Vector3 m_scale;
        private Vector3 m_rotation;
        public int EndFrame => m_endFrame;

        FrameDataManager m_manager;
        private Collider[] m_colliders; //colliders of object and all children

        public void Initialize(FrameDataManager manager, int groupID, string onHit, int endFrame, HitboxShapes shape, Vector3 scale, Vector3 rotation)
        {
            m_groupID = groupID;
            m_manager = manager;
            m_onHit = onHit;
            m_endFrame = endFrame;
            m_shape = shape;
            m_scale = scale;
            m_rotation = rotation;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            //m_colliders = GetComponentsInChildren<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 lossyScale = transform.lossyScale;
            Quaternion parentRotation = Quaternion.identity;

            if (transform.parent != null)
            {
                lossyScale = transform.parent.lossyScale;
                parentRotation = transform.parent.rotation;
            }

            int hitCount = 0;

            switch (m_shape)
            {
                case HitboxShapes.Rectangle:

                    hitCount = HitboxUtil.OverlapBox(transform.position, Vector3.Scale((m_scale / 2), lossyScale), parentRotation * Quaternion.Euler(m_rotation), 0);

                    CheckCollision(hitCount);
                    
                    break;

                case HitboxShapes.Sphere:
                    
                    hitCount = HitboxUtil.OverlapSphere(transform.position, m_scale.x / 2, 0);
                    CheckCollision(hitCount);
                    
                    break;

                case HitboxShapes.Capsule:

                    Quaternion rot = parentRotation * Quaternion.Euler(m_rotation);

                    Vector3 axis = rot * Vector3.up;

                    float radius = (m_scale.x / 2) * Mathf.Max(lossyScale.x, lossyScale.z);

                    float height = (Mathf.Clamp(m_scale.y * 2f, m_scale.x, Mathf.Infinity)) * lossyScale.y;
                    float halfCylinder = (height / 2f) - radius;

                    Vector3 point0 = transform.position + axis * halfCylinder;
                    Vector3 point1 = transform.position - axis * halfCylinder;

                    hitCount = HitboxUtil.OverlapCapsule(point0, point1, radius, 0);

                    CheckCollision(hitCount);

                    break;
                default:
                    break;
            }

            
        }

        private void CheckCollision(int hitCount)
        {
            if (hitCount == 0) return;

            for (int i = 0; i < hitCount; i++)
            {

                Collider hitCollider = HitboxUtil.Get(i);

                HitboxCollider hitColliderScript = hitCollider.GetComponent<HitboxCollider>();

                if (!hitColliderScript) continue;

                if (m_manager.CheckHit(m_groupID, hitCollider))
                {
                    //invoke hitbox onhit event
                    m_manager.InvokeEvent(m_onHit);

                    //invoke object hit event
                    hitColliderScript.Invoke();
                }
            }
        }
    }
}


