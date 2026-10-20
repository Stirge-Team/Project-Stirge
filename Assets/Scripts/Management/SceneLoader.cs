using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Stirge.Management
{
    public class SceneLoader : MonoBehaviour
    {
        [System.Serializable]
        public struct sceneData
        {
            public string m_sceneName;
            public LoadSceneMode m_loadMode;
        }

        public sceneData[] m_scenes;

        void Awake()
        {
            // get names of all current scenes
            int activeSceneCount = SceneManager.sceneCount;
            string[] activeScenes = new string[SceneManager.sceneCount];
            for (int i = 0; i < activeSceneCount; i++)
            {
                activeScenes[i] = SceneManager.GetSceneAt(i).name;
            }

            // do not load scenes that are already loaded
            for (int i = 0, count = m_scenes.Length; i < count; i++)
            {
                sceneData scene = m_scenes[i];
                if (!activeScenes.Contains(scene.m_sceneName))
                    SceneManager.LoadScene(scene.m_sceneName, scene.m_loadMode);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Load Scenes")]
        public void LoadScenesInEditor()
        {
            try
            {
                foreach (var scene in m_scenes)
                {
                    string[] guids = AssetDatabase.FindAssets(scene.m_sceneName, new string[] { "Assets/Scenes" });
                    for (int i = 0, sceneCount = guids.Length; i < sceneCount; i++)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                        if (path.Contains(".unity"))
                            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load scenes!\n" + e.Message, this);
                return;
            }

            Debug.Log("Successfully loaded additional scenes into Editor!");
        }
#endif
    }
}
