#if UNITY_EDITOR

using UnityEngine;
using static FrameFighter2.Data.HitboxData;
using static FrameFighter2.Viewer.HitboxGizmoPreview;

namespace FrameFighter2.Viewer 
{

    public class HitboxGizmoObject : MonoBehaviour
    {
        [ExecuteAlways]
        private void OnDrawGizmos()
        {
            if (PreviewObjects.Count == 0) DestroyImmediate(gameObject);

            for (int i = 0; i < PreviewObjects.Count; i++)
            {
                //Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Preview current = PreviewObjects[i];

                if (current.ObjectExists())
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(current.PreviewObject.transform.position, current.PreviewObject.transform.parent.rotation * Quaternion.Euler(current.Rot), current.PreviewObject.transform.parent.lossyScale);

                    switch (current.Shape)
                    {
                        case HitboxShapes.Capsule:

                            Quaternion rot = current.PreviewObject.transform.parent.rotation * Quaternion.Euler(current.Rot);

                            Vector3 axis = rot * Vector3.up;

                            Vector3 scale = current.PreviewObject.transform.parent.lossyScale;

                            //float radius = (current.Size.x / 2);
                            float radius = (current.Size.x / 2) * Mathf.Max(scale.x, scale.z);

                            //float height = Mathf.Max(current.Size.y * 2f, radius * 2f);
                            float height = (Mathf.Clamp(current.Size.y * 2f, current.Size.x, Mathf.Infinity)) * scale.y;
                            float halfCylinder = (height / 2f) - radius;

                            Vector3 point1 = current.PreviewObject.transform.position + axis * halfCylinder;
                            Vector3 point2 = current.PreviewObject.transform.position - axis * halfCylinder;

                            Gizmos.color = Color.green;

                            Gizmos.DrawWireSphere(point1, radius);
                            Gizmos.DrawWireSphere(point2, radius);

                            //boring math to make sure the lines line (heh) up when the capsule is rotated

                            Vector3 side1 = Vector3.Cross(axis, Vector3.up);
                            if (side1.sqrMagnitude < 0.001f)
                                side1 = Vector3.Cross(axis, Vector3.right);

                            side1.Normalize();
                            Vector3 side2 = Vector3.Cross(axis, side1);

                            Gizmos.DrawLine(point1 + side1 * radius, point2 + side1 * radius);
                            Gizmos.DrawLine(point1 - side1 * radius, point2 - side1 * radius);

                            Gizmos.DrawLine(point1 + side2 * radius, point2 + side2 * radius);
                            Gizmos.DrawLine(point1 - side2 * radius, point2 - side2 * radius);

                            break;

                        case HitboxShapes.Rectangle:

                            Gizmos.matrix = matrix;

                            Gizmos.DrawWireCube(Vector3.zero, current.Size);

                            break;

                        case HitboxShapes.Sphere:

                            Gizmos.matrix = matrix;

                            Gizmos.DrawWireSphere(Vector3.zero, current.Size.x / 2);

                            break;
                        default:
                            //do notn'
                            break;
                    }
                }  
                else
                {
                    PreviewObjects.Remove(PreviewObjects[i]);
                }

                //reset matrix
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }

}

#endif
