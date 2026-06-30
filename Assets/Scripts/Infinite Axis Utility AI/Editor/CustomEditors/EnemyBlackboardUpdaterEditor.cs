using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using EG = UnityEditor.EditorGUI;
using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI
{
    using Callbacks;
    using Stirge.Serialization;

    [CustomEditor(typeof(EnemyBlackboardUpdater))]
    public class EnemyBlackboardUpdaterEditor : Editor
    {
        private const string SerializedEnemyPropertyName = "m_enemy";
        private const string SerializedCallbacksPropertyName = "m_callbacks";

        private readonly Dictionary<Object, Editor> m_editors = new();

        private void OnDestroy()
        {
            foreach (Editor editor in m_editors.Values)
            {
                DestroyImmediate(editor);
            }
        }

        public override void OnInspectorGUI()
        {
            // show Script fields for quick editing
            using (new EditorGUI.DisabledScope(true))
            {
                if (target is ScriptableObject so)
                {
                    EGL.ObjectField("Script", MonoScript.FromScriptableObject(so), target.GetType(), false);
                    EGL.ObjectField("Editor", MonoScript.FromScriptableObject(this), GetType(), false);
                }
                else
                {
                    EGL.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), target.GetType(), false);
                    EGL.ObjectField("Editor", MonoScript.FromScriptableObject(this), GetType(), false);
                }
            }

            if (!PrefabUtility.IsPartOfPrefabAsset(target))
            {
                EGL.TextArea("This Component cannot be edited on Prefab instances.");
                return;
            }

            // Draw Enemy property field
            EG.BeginChangeCheck();
            SerializedProperty enemyProperty = serializedObject.FindProperty(SerializedEnemyPropertyName);
            EGL.PropertyField(enemyProperty);
            if (EG.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            // Draw Callbacks
            SerializedProperty callbacksProperty = serializedObject.FindProperty(SerializedCallbacksPropertyName);

            EGL.BeginVertical(GUI.skin.window);

            // draw each of the callbacks
            for (int i = 0, count = callbacksProperty.arraySize; i < count; i++)
            {
                SerializedProperty callbackProperty = callbacksProperty.GetArrayElementAtIndex(i);
                Object objectValue = (CallbackBinding_Base)callbackProperty.objectReferenceValue;  
                
                if (!m_editors.TryGetValue(objectValue, out Editor editor))
                {
                    editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                    m_editors.Add(objectValue, editor);
                }

                EGL.BeginVertical(GUI.skin.box);

                EGL.LabelField(GetUIName(objectValue.GetType()));

                editor.OnInspectorGUI();

                if (GUILayout.Button("Remove Callback"))
                {
                    DestroyImmediate(objectValue, true);
                    SerializedPropertyHelper.CompletelyRemove(callbacksProperty, i);

                    --i;
                    count = callbacksProperty.arraySize;

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                EGL.EndVertical();
            }

            EGL.Separator();

            if (GUILayout.Button("Add Callback"))
            {
                AddCallback(callbacksProperty);
            }

            EGL.EndVertical();

            EGL.Separator();
        }

        private void AddCallback(SerializedProperty callbacksProperty)
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> callbackTypes = SerializableCallbackTypesCollection.callbackTypes;

            for (int i = 0, count = callbackTypes.Count; i < count; i++)
            {
                Type type = callbackTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    Type serializableCallbackType = SerializableCallbackTypesCollection.GetSerializableCallbackType(type);
                    ScriptableObject instance = CreateInstance(serializableCallbackType);
                    instance.name = uiName.Replace(" ", string.Empty);

                    AssetDatabase.AddObjectToAsset(instance, target);

                    int index = callbacksProperty.arraySize++;
                    callbacksProperty.GetArrayElementAtIndex(index).objectReferenceValue = instance;

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                });
            }

            genericMenu.ShowAsContext();
        }

        public static string GetUIName(Type type)
        {
            if (!type.IsGenericType)
            {
                // "object" name not to confuse it with UnityEngine.Object.
                return type == typeof(object) ? "object" : GetNameWithSpaces(type.Name);
            }

            string genericName = type.Name;
            genericName = genericName[..genericName.IndexOf("`", StringComparison.Ordinal)];
            var stringBuilder = new StringBuilder(GetNameWithSpaces(genericName));
            stringBuilder.Append(" <");

            Type[] genericParameters = type.GetGenericArguments();

            for (int i = 0, count = genericParameters.Length; i < count; ++i)
            {
                stringBuilder.Append(GetUIName(genericParameters[i]));

                if (i < count - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append('>');

            return stringBuilder.ToString();
        }

        private static string GetNameWithSpaces(string typeName)
        {
            return Regex.Replace(typeName, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        }
    }
}
