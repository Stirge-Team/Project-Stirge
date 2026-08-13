using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;

namespace Stirge.InfiniteAxis.CustomEditors
{
    using Core;

    [CustomEditor(typeof(UtilityEnemy))]
    public class UtilityEnemyEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EGL.Separator();

            UtilityEnemy enemy = (UtilityEnemy)target;
            SerializedProperty actorProperty = serializedObject.FindProperty("m_actor");

            EGL.LabelField("Actor Debug Information", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (actorProperty != null && actorProperty.managedReferenceValue is Actor actor && actor != null)
            {
                if (GUILayout.Button("Destroy Actor"))
                {
                    enemy.ClearAIComponents();
                }
                else
                {
                    string[] data = new string[0];
                    try
                    {
                        Actor.GetDebugInfo(actor, ref data);
                    }
                    catch (System.NullReferenceException)
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EGL.TextField("Actor is kinda fucked up!");
                        }
                    }
                    for (int i = 0, count = data.Length; i < count; i++)
                    {
                        EGL.LabelField(data[i]);
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Create Debug Actor"))
                {
                    enemy.InitialiseAIComponents();
                }
                else
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EGL.TextField("No Actor!");
                    }
                }
            }

            EditorGUI.indentLevel--;
        }
    }
}
