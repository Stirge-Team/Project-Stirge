using UnityEngine;

namespace Stirge.UtilityAI.CustomEditors
{
    using Blackboard;
    using UnityEditor;

    //[CustomEditor(typeof(EnemyBlackboard))]
    public class EnemyBlackboardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SerializedProperty floatDictProp = serializedObject.FindProperty("m_floatDict");
            SerializedProperty vector3DictProp = serializedObject.FindProperty("m_vector3Dict");
        }
    }
}
