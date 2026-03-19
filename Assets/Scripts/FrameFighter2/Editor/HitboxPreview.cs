using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static FrameFighter2.Data.HitboxData;

namespace FrameFighter2.Viewer
{

    [InitializeOnLoad]
    public static class HitboxPreview
    {
        static List<GameObject> _previews = new List<GameObject>();
        private static Material[] _material = new Material[2];
        
        static Material[] HitboxMaterial
        {
            get
            {
                if (_material[0] == null)
                {
                    _material[0] = CreateTempMaterial(0);
                    SceneView.RepaintAll();
                }
                if (_material[1] == null)
                {
                    _material[1] = CreateTempMaterial(1);
                    SceneView.RepaintAll();
                }
                return _material;
            }
        }

        static Material CreateTempMaterial(int colour) //colour determines if the material should be the "active" or "inactive" colour
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            Material tempMat = new Material(shader);
            //transparency and colour
            tempMat.SetFloat("_Surface", 1); // Transparent
            tempMat.SetFloat("_Blend", 0);   // Alpha blend
            tempMat.SetFloat("_ZWrite", 0);
            Color boxColour = (colour == 0) ? new Color(1f, 0f, 0f, 0.35f) : new Color(0f, 1f, 0f, 0.35f); //if colour = 0, red. if colour = 1, green.
            tempMat.SetColor("_BaseColor", boxColour);
            // Lighting cleanup
            tempMat.SetFloat("_Metallic", 0f);
            tempMat.SetFloat("_Smoothness", 0f);
            // disable specularhighlights
            tempMat.SetFloat("_SpecularHighlights", 0f);
            // disable reflections
            tempMat.SetFloat("_EnvironmentReflections", 0f);
            // disable recieve shadows
            tempMat.SetFloat("_ReceiveShadows", 0f);

            // FORCE URP TO REBUILD INTERNAL STATE - I HATE UNITY!!!!!
            tempMat.shader = shader;

            tempMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            return tempMat;
        } 

        public static void CreatePreview(Vector3 pos, Vector3 rot, Vector3 scale, HitboxShapes shape, bool isActive = false)
        {
            PrimitiveType previewToCreate = shape switch
            {
                HitboxShapes.Rectangle => PrimitiveType.Cube,
                HitboxShapes.Capsule => PrimitiveType.Capsule,
                HitboxShapes.Sphere => PrimitiveType.Sphere,
                _ => throw new System.NotImplementedException(),
            };

            GameObject obj = GameObject.CreatePrimitive(previewToCreate);
            obj.hideFlags = HideFlags.HideAndDontSave;
            _previews.Add(obj);

            obj.GetComponent<Renderer>().sharedMaterial = HitboxMaterial[(isActive) ? 1 : 0];
            obj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            obj.transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(rot));
            obj.transform.localScale = scale;
        }

        public static void CreatePreview(Transform parent, Vector3 pos, Vector3 rot, Vector3 scale, HitboxShapes shape, bool isActive = false)
        {

            PrimitiveType previewToCreate = shape switch
            {
                HitboxShapes.Rectangle => PrimitiveType.Cube,
                HitboxShapes.Capsule => PrimitiveType.Capsule,
                HitboxShapes.Sphere => PrimitiveType.Sphere,
                _ => throw new System.NotImplementedException(),
            };

            GameObject obj = GameObject.CreatePrimitive(previewToCreate);
            obj.name = "HitBoxPreview";

            obj.hideFlags = HideFlags.HideAndDontSave;
            _previews.Add(obj);

            obj.GetComponent<Renderer>().sharedMaterial = HitboxMaterial[(isActive) ? 1 : 0];
            obj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //obj.GetComponent<Renderer>().material.color = isActive ? new(0f, 1f, 0f, 0.35f) : new(1f, 0f, 0f, 0.35f);

            obj.transform.parent = parent;
            obj.transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(rot));
            obj.transform.localScale = scale;

            HitboxGizmoPreview.AddPreview(obj, shape, pos, scale, rot);
            
            switch (previewToCreate)
            {
                case PrimitiveType.Capsule:
                    //obj.AddComponent<CapsulePreview>().Initialize(obj.transform.position, rot, scale.x, scale.y);
                    break;
                default:
                    break;
            }

        }

        public static void DestroyAllPreviews()
        {
            foreach (GameObject obj in _previews)
            {
                //obj.GetComponent<Renderer>().material = null;

                Object.DestroyImmediate(obj);
                
            }
        }
    }


    [InitializeOnLoad]
    static class PreviewCleanup
    {
        static PreviewCleanup()
        {
            EditorApplication.playModeStateChanged += _ =>
            {
                HitboxPreview.DestroyAllPreviews();
            };

            AssemblyReloadEvents.beforeAssemblyReload += HitboxPreview.DestroyAllPreviews;
            
        }
    }
}


