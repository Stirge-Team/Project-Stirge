#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using static FrameFighter2.Data.HitboxData;

namespace FrameFighter2.Viewer
{
    public class HitboxGizmoPreview
    {
        public class Preview
        {
            private GameObject m_previewObject;
            public GameObject PreviewObject => m_previewObject;
            private HitboxShapes m_shape;
            public HitboxShapes Shape => m_shape;
            private Vector3 m_pos;
            public Vector3 Pos => m_pos;
            private Vector3 m_size;
            public Vector3 Size => m_size;
            private Vector3 m_rot;
            public Vector3 Rot => m_rot;

            public Preview(GameObject previewObject, HitboxShapes shape, Vector3 pos, Vector3 scale, Vector3 rot)
            {
                m_previewObject = previewObject;
                m_shape = shape;
                m_pos = pos;
                m_size = scale;
                m_rot = rot;
            }

            public bool ObjectExists()
            {
                return m_previewObject != null;
            }
        }

        public static List<Preview> PreviewObjects = new(0);
        private static GameObject m_gizmoObj;
        static GameObject GizmoObject
        {
            get
            {
                if(m_gizmoObj == null)
                {
                    m_gizmoObj = new("Gizmo Object");
                    m_gizmoObj.hideFlags = HideFlags.DontSave;
                    m_gizmoObj.AddComponent<HitboxGizmoObject>();
                }
                return m_gizmoObj;
            }
        }

        public static void AddPreview(GameObject previewHitbox, HitboxShapes shape, Vector3 pos, Vector3 scale, Vector3 rot)
        {
            Preview newPreview = new(previewHitbox, shape, pos, scale, rot);

            PreviewObjects.Add(newPreview);

            GizmoObject.SetActive(true); 
        }

    }
}
#endif

