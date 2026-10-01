using Stirge.Environment;
using UnityEditor;
using UnityEngine;

namespace Stirge
{
    public static class TriggerBoxMenu
    {
        const string m_triggerBoxObjectMenuRoot = "GameObject/Simple Trigger Box/";

        
        private static GameObject CreateBoxRoot<T>(string name = null) where T : SimpleTriggerBox
        {
            var newGO = new GameObject();
            newGO.AddComponent<BoxCollider>();
            newGO.AddComponent<T>();
            newGO.name = name != null ? name : typeof(T).Name;
            return newGO;
        }

        //[MenuItem(m_triggerBoxObjectMenuRoot + "Simple Trigger Box")]
        static void CreateNewTriggerBox()
        {
            CreateBoxRoot<SimpleTriggerBox>();
        }

        [MenuItem(m_triggerBoxObjectMenuRoot + "Death Box")]
        static void CreateNewDeathBox()
        {
            CreateBoxRoot<DeathTriggerBox>();
        }

        [MenuItem(m_triggerBoxObjectMenuRoot + "Event Box")]
        static void CreateEventBox()
        {
            CreateBoxRoot<EventTriggerBox>();
        }
        
        [MenuItem(m_triggerBoxObjectMenuRoot + "ReTrigger Box")]
        static void CreateReTriggerBox()
        {
            CreateBoxRoot<ReTriggerBox>();
        }
    }
}
