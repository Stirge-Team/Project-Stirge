using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Environment
{
    [RequireComponent(typeof(Collider))]
    public class SimpleTriggerBox : MonoBehaviour
    {
        #region Vars
        private Collider[] m_collider;
        public Collider[] Triggers { get { return m_collider; } }
        private List<GameObject> m_colliderPreviews;
        [SerializeField, Tooltip("Forces any attached collider components to become trigger boxes. Keep this as true unless you have a specific collider setup configured.")]
        private bool m_forceTrigger = true;
        #endregion
        #region Setup
        void Awake()
        {
            SetupForGizmos();
        }
        void Reset()
        {
            SetupForGizmos(true);
            //ClearPreviews();
        }
        private void SetupForGizmos(bool enableForce = false)
        {
            CollectTriggers();
            if (m_forceTrigger && enableForce) ForceTriggers();
        }
        private void CollectTriggers()
        {
            var tmp = GetComponents<Collider>();
            if (tmp.Length < 1)
            {
                Debug.LogError("There are no colliders attached to this object. Somehow.");
                enabled = false;
                return;
            }
            m_collider = tmp;
        }
        private void ForceTriggers()
        {
            foreach (var coli in m_collider)
                if (!coli.isTrigger) coli.isTrigger = true;
        }
        #endregion
        #region Trigger Interactions
        //Trigger Enter
        public virtual void OnTriggerEnter(Collider collider)
        {
            Debug.Log($"{collider.name} has entered {name} collider.");
        }
        //Trigger Stay
        public virtual void OnTriggerStay(Collider collider)
        {
            Debug.Log($"{collider.name} is within the {name} collider.");
        }
        //Trigger Exit
        public virtual void OnTriggerExit(Collider collider)
        {
            Debug.Log($"{collider.name} has exited {name} collider.");
        }
        #endregion
        #region Previews
        /*
                public void RenderPreviews()
                {
                    ClearPreviews();

                    foreach (var coli in Triggers)
                    {
                        //newPreview.transform.localScale = Vector3.Cross(coli.transform.localScale, coli.bounds.size);
                        GameObject newPreview = GameObject.CreatePrimitive(coli.GetType().Name switch
                        {
                            nameof(BoxCollider) => PrimitiveType.Cube,
                            nameof(SphereCollider) => PrimitiveType.Sphere,
                            nameof(CapsuleCollider) => PrimitiveType.Capsule,
                            _ => throw new System.NotImplementedException()
                        });
                        DestroyImmediate(newPreview.GetComponent<Collider>()); //remove redundent collider

                        newPreview.transform.SetParent(transform, true);
                        newPreview.transform.rotation = transform.rotation;
                        Debug.Log($"Collider center: {coli.bounds.center}");
                        newPreview.transform.position = coli.bounds.center;// Vector3Div(coli.bounds.center, transform.localScale);
                        Debug.Log($"Collider scale: {coli.bounds.size}");
                        newPreview.transform.localScale = coli.bounds.size;//Vector3Div(coli.bounds.size, transform.localScale);

                        //shamelessly stolen from framefighter2, tyvm Aidan
                        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                        Material previewMat = new(shader);
                        previewMat.SetFloat("_Surface", 1); // Transparent
                        previewMat.SetFloat("_Blend", 0);   // Alpha blend
                        previewMat.SetFloat("_ZWrite", 0);
                        Color boxColour = new Color(1f, 0f, 0f, 0.35f);
                        previewMat.SetColor("_BaseColor", boxColour);
                        // Lighting cleanup
                        previewMat.SetFloat("_Metallic", 0f);
                        previewMat.SetFloat("_Smoothness", 0f);
                        // disable specularhighlights
                        previewMat.SetFloat("_SpecularHighlights", 0f);
                        // disable reflections
                        previewMat.SetFloat("_EnvironmentReflections", 0f);
                        // disable recieve shadows
                        previewMat.SetFloat("_ReceiveShadows", 0f);
                        previewMat.shader = shader;
                        previewMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        newPreview.GetComponent<MeshRenderer>().material = previewMat;
                        SceneView.RepaintAll();

                        m_colliderPreviews.Add(newPreview);
                    }
                }

                private void ClearPreviews()
                {
                    foreach (var trigger in m_colliderPreviews)
                    {
                        DestroyImmediate(trigger);
                    }
                    m_colliderPreviews.Clear();
                }*/

        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.antiqueWhite;
            //Gizmos.DrawCube(Triggers[0].bounds.center, Triggers[0].bounds.size);
        }
        #endregion
        private Vector3 Vector3Mult(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.x * rhs.x,
                lhs.y * rhs.y,
                lhs.z * rhs.z
            );
        }
        private Vector3 Vector3Div(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.x / rhs.x,
                lhs.y / rhs.y,
                lhs.z / rhs.z
            );
        }
    }
}
