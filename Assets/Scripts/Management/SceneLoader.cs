using UnityEngine;
using UnityEngine.SceneManagement;

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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (var scene in m_scenes)
            {
                SceneManager.LoadScene(scene.m_sceneName, scene.m_loadMode);
            }
        }

        // Update is called once per frame
        void Update() { }
    }
}
