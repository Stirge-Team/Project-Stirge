using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Stirge.Environment
{
    /// <summary>
    /// The base of all trigger box classes - not used itself
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SimpleTriggerBox : MonoBehaviour
    {
        #region Vars
        private Collider[] m_collider;
        public Collider[] Triggers { get { return m_collider; } }
        private List<GameObject> m_colliderPreviews;
        [Flags]
        protected enum ForceTriggerMethod
        {
            None = 0, //forces no colliders to be triggers
            BaseObject = 1, //force triggers on the base object
            ChildObjects = 2 //force triggers on child objects
        }
        [Header("Trigger Settings")]
        [SerializeField, Tooltip("Forces any attached collider components to become trigger boxes. Keep this as true unless you have a specific collider setup configured.")]
        private ForceTriggerMethod m_forceTrigger = (ForceTriggerMethod)1;
        private enum TriggerType
        {
            SingleUse,
            NoRepeatObjects,
            Repeatable,
            Off
        };
        [System.Serializable]
        private struct TriggerActivator
        {
            [SerializeField]
            private TriggerType m_repeatable;
            private bool m_triggered;
            private List<Transform> m_collidedObjects;
            public delegate void CollisionCallback(Collider collider);
            public CollisionCallback _collisionCallback;
            public void Trigger(Collider collider)
            {
                switch (m_repeatable)
                {
                    case TriggerType.SingleUse:
                        if (m_triggered) return;
                        break;
                    case TriggerType.NoRepeatObjects:
                        if (PreviousCollider(collider)) return;
                        _collisionCallback(collider);
                        break;
                    case TriggerType.Off: return;
                }
                _collisionCallback(collider);
                m_triggered = true;
            }
            public bool PreviousCollider(Collider collider)
            {
                if (m_collidedObjects.Contains(collider.transform)) return true;
                else
                {
                    m_collidedObjects.Add(collider.transform);
                    return false;
                }
            }
            public void Reenable()
            {
                if (!m_triggered && m_repeatable != TriggerType.Off) Debug.Log($"{this} trigger not hit yet. If this being called multiple times, please check your calls and trigger placement in scene");
                m_triggered = false;
            }
        }
        [SerializeField]
        private TriggerActivator m_EntryTrigger;
        [SerializeField]
        private TriggerActivator m_StayTrigger;
        [SerializeField]
        private TriggerActivator m_ExitTrigger;
        #endregion
        #region Setup
        void Awake()
        {
            m_EntryTrigger._collisionCallback += EnterFunc;
            m_StayTrigger._collisionCallback += StayFunc;
            m_ExitTrigger._collisionCallback += ExitFunc;
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
            if (m_forceTrigger > 0 || enableForce) ForceTriggers();
        }
        private void CollectTriggers()
        {
            var colliderList = new List<Collider>();

            if (m_forceTrigger.HasFlag(ForceTriggerMethod.BaseObject))
            {
                colliderList.AddRange(GetComponents<Collider>());
            }
            if(m_forceTrigger.HasFlag(ForceTriggerMethod.ChildObjects))
            {
                colliderList.AddRange(GetComponentsInChildren<Collider>());
            }

            m_collider = colliderList.ToArray();
        }
        private void ForceTriggers()
        {
            foreach (var coli in m_collider)
                if (!coli.isTrigger) coli.isTrigger = true;
        }
        #endregion
        #region Trigger Interactions
        //Trigger Enter
        public void OnTriggerEnter(Collider collider)
        {
            m_EntryTrigger.Trigger(collider);
        }
        //Trigger Stay
        public void OnTriggerStay(Collider collider)
        {
            m_StayTrigger.Trigger(collider);
        }
        //Trigger Exit
        public void OnTriggerExit(Collider collider)
        {
            m_ExitTrigger.Trigger(collider);
        }
        #endregion
        protected virtual void EnterFunc(Collider collider)
        {
            Debug.Log($"{collider.name} has entered {name} collider.");
        }
        protected virtual void StayFunc(Collider collider)
        {
            Debug.Log($"{collider.name} is within the {name} collider.");
        }
        protected virtual void ExitFunc(Collider collider)
        {
            Debug.Log($"{collider.name} has exited {name} collider.");
        }
        [Flags]
        public enum SelectTriggerEvent
        {
            Entry = 1,
            Stay = 2,
            Exit = 4
        }
        public void ReenableTriggers(SelectTriggerEvent triggers = (SelectTriggerEvent)7)
        {
            if (triggers.HasFlag(SelectTriggerEvent.Entry))
                m_EntryTrigger.Reenable();
            if (triggers.HasFlag(SelectTriggerEvent.Stay))
                m_StayTrigger.Reenable();
            if (triggers.HasFlag(SelectTriggerEvent.Exit))
                m_ExitTrigger.Reenable();
        }
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
    }
}
