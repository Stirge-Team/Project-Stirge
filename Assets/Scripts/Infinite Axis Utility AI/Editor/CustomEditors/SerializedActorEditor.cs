using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Stirge.Serialization;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
{
    using EditorTools;
    using Serialization;

    [CustomEditor(typeof(SerializedActor))]
    public class SerializedActorEditor : Editor
    {
        private const string SerializedActionsPropertyName = "m_serializedActions";
        private const string SerializedAxesPropertyName = "m_serializedAxes";
        private const string SerializedAxisIndicesPropertyName = "m_axisIndices";
        private const string AxesPropertyName = "m_axes";

        private readonly Dictionary<Object, Editor> m_editors = new();

        private static bool s_axesFoldout = true;
        private static bool s_actionsFoldout = true;

        private void OnDestroy()
        {
            foreach (Editor editor in m_editors.Values)
            {
                DestroyImmediate(editor);
            }
        }

        public override void OnInspectorGUI()
        {
            SerializedProperty actionsProperty = serializedObject.FindProperty(SerializedActionsPropertyName);
            SerializedProperty axesProperty = serializedObject.FindProperty(SerializedAxesPropertyName);
            SerializedProperty axisIndicesProperty = serializedObject.FindProperty(SerializedAxisIndicesPropertyName);

            s_axesFoldout = EditorGUILayout.Foldout(s_axesFoldout, "Axis");
            if (s_axesFoldout)
            {
                EditorGUILayout.BeginVertical(GUI.skin.window);

                for (int i = 0, count = axesProperty.arraySize; i < count; ++i)
                {
                    SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(i);
                    var objectValue = (SerializedAxis_Base)axisProperty.objectReferenceValue;

                    if (!m_editors.TryGetValue(objectValue, out Editor editor))
                    {
                        editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                        m_editors.Add(objectValue, editor);
                    }

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.LabelField(GetUIName(objectValue.axisType), EditorStyles.boldLabel);
                    objectValue.name = EditorGUILayout.TextField("Name", objectValue.name);

                    editor.OnInspectorGUI();

                    if (GUILayout.Button("Remove Axis"))
                    {
                        DestroyImmediate(objectValue, true);
                        SerializedPropertyHelper.CompletelyRemove(axesProperty, i);
                        RemoveAxisIndex(axisIndicesProperty, i);

                        --i;
                        count = axesProperty.arraySize;

                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Add Axis"))
                {
                    AddAxis(axesProperty);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            s_actionsFoldout = EditorGUILayout.Foldout(s_actionsFoldout, "Actions");
            if (s_actionsFoldout)
            {
                EditorGUILayout.BeginVertical(GUI.skin.window);

                for (int i = 0, count = actionsProperty.arraySize; i < count; ++i)
                {
                    SerializedProperty actionProperty = actionsProperty.GetArrayElementAtIndex(i);
                    var objectValue = (SerializedAction_Base)actionProperty.objectReferenceValue;

                    if (!m_editors.TryGetValue(objectValue, out Editor editor))
                    {
                        editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                        m_editors.Add(objectValue, editor);
                    }

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    EditorGUILayout.LabelField(GetUIName(objectValue.actionType), EditorStyles.boldLabel);
                    objectValue.name = EditorGUILayout.TextField("Name", objectValue.name);

                    editor.OnInspectorGUI();

                    EditorGUILayout.LabelField("Axes");

                    SerializedProperty axes = axisIndicesProperty.GetArrayElementAtIndex(i).FindPropertyRelative(AxesPropertyName);
                    MakeAxesMenu(axes, axesProperty);

                    if (GUILayout.Button("Remove Action"))
                    {
                        DestroyImmediate(objectValue, true);
                        SerializedPropertyHelper.CompletelyRemove(actionsProperty, i);
                        SerializedPropertyHelper.CompletelyRemove(axisIndicesProperty, i);

                        --i;
                        count = actionsProperty.arraySize;

                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Add Action"))
                {
                    AddAction(actionsProperty, axisIndicesProperty);
                }

                EditorGUILayout.EndVertical();
            }
        }

        #region Add Properties
        private void AddAxis(SerializedProperty axesProperty)
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> axisTypes = SerializedAxisTypesCollection.axisTypes;

            for (int i = 0, count = axisTypes.Count; i < count; ++i)
            {
                Type type = axisTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    Type serializedAxisType = SerializedAxisTypesCollection.GetSerializedAxisType(type);
                    ScriptableObject instance = CreateInstance(serializedAxisType);
                    instance.name = uiName.Replace(" ", string.Empty);

                    AssetDatabase.AddObjectToAsset(instance, target);

                    int index = axesProperty.arraySize++;
                    axesProperty.GetArrayElementAtIndex(index).objectReferenceValue = instance;

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                });
            }

            genericMenu.ShowAsContext();
        }

        private void AddAction(SerializedProperty actionsProperty, SerializedProperty axisIndicesProperty)
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> actionTypes = SerializedActionTypesCollection.actionTypes;

            for (int i = 0, count = actionTypes.Count; i < count; ++i)
            {
                Type type = actionTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    Type serializedActionType = SerializedActionTypesCollection.GetSerializedActionType(type);
                    ScriptableObject instance = CreateInstance(serializedActionType);
                    instance.name = uiName.Replace(" ", string.Empty);

                    AssetDatabase.AddObjectToAsset(instance, target);

                    int index = actionsProperty.arraySize++;
                    actionsProperty.GetArrayElementAtIndex(index).objectReferenceValue = instance;
                    ++axisIndicesProperty.arraySize;
                    axisIndicesProperty.GetArrayElementAtIndex(axisIndicesProperty.arraySize - 1).FindPropertyRelative(AxesPropertyName).ClearArray();

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                });
            }

            genericMenu.ShowAsContext();
        }

        private static void RemoveAxisIndex(SerializedProperty axisIndices, int index)
        {
            for (int actionIndex = 0, actionCount = axisIndices.arraySize; actionIndex < actionCount; ++actionIndex)
            {
                SerializedProperty axes = axisIndices.GetArrayElementAtIndex(actionIndex).FindPropertyRelative(AxesPropertyName);

                for (int axisIndex = 0, axisCount = axes.arraySize; axisIndex < axisCount; ++axisIndex)
                {
                    int axis = axes.GetArrayElementAtIndex(axisIndex).intValue;

                    if (axis > index)
                    {
                        --axes.GetArrayElementAtIndex(axisIndex).intValue;
                    }
                    else if (axis == index)
                    {
                        SerializedPropertyHelper.CompletelyRemove(axes, axisIndex);
                        --axisIndex;
                        axisCount = axes.arraySize;
                    }
                }
            }
        }
        #endregion

        #region Menus
        private void MakeAxesMenu(SerializedProperty axes, SerializedProperty axesProperty)
        {
            static bool HasValue(SerializedProperty property, int value)
            {
                for (int i = 0, count = property.arraySize; i < count; ++i)
                {
                    if (property.GetArrayElementAtIndex(i).intValue == value)
                    {
                        return true;
                    }
                }

                return false;
            }
            
            for (int i = 0, count = axes.arraySize; i < count; ++i)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                var axis = (SerializedAxis_Base)axesProperty.GetArrayElementAtIndex(axes.GetArrayElementAtIndex(i).intValue).objectReferenceValue;

                EditorGUILayout.LabelField($"{axis.name} : {GetUIName(axis.axisType)}");

                if (GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    SerializedPropertyHelper.CompletelyRemove(axes, i);
                    --i;
                    count = axes.arraySize;

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Axis"))
            {
                var genericMenu = new GenericMenu();

                for (int i = 0, count = axesProperty.arraySize; i < count; ++i)
                {
                    if (HasValue(axes, i))
                    {
                        continue;
                    }

                    int index = i;
                    var axis = (SerializedAxis_Base)axesProperty.GetArrayElementAtIndex(index).objectReferenceValue;

                    genericMenu.AddItem(new GUIContent($"{axis.name} : {GetUIName(axis.axisType)}"), false, () =>
                    {
                        int addIndex = axes.arraySize++;
                        axes.GetArrayElementAtIndex(addIndex).intValue = index;

                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                    });
                }

                genericMenu.ShowAsContext();
            }
        }
        #endregion

        #region NameFormatting
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
        #endregion
    }
}
