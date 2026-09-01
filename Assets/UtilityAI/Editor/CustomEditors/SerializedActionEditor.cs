using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
{
    using EditorTools;
    using Serialization;

    [CustomEditor(typeof(SerializedAction))]
    public class SerializedActionEditor : Editor
    {
        #region Property Names
        private const string s_scoreScalingPropertyName = "m_scoreScaling";
        private const string s_displayNamePropertyName = "m_displayName";
        private const string s_actionTypePropertyName = "m_actionType";
        private const string s_timelineAssetPropertyName = "m_timeline";
        private const string s_damagePropertyName = "m_damage";
        private const string s_rangePropertyName = "m_range";
        private const string s_statusesPropertyName = "m_statuses";
        private const string s_conditionsPropertyName = "m_conditions";
        private const string s_scoringMethodsPropertyName = "m_scoringMethods";
        #endregion

        #region Serialized Properties
        private SerializedProperty m_scoreScalingProperty;
        private SerializedProperty m_displayNameProperty;
        private SerializedProperty m_actionTypeProperty;
        private SerializedProperty m_timelineAssetProperty;
        private SerializedProperty m_damageProperty;
        private SerializedProperty m_rangeProperty;
        private SerializedProperty m_statusesProperty;
        private SerializedProperty m_conditionsProperty;
        private SerializedProperty m_scoringMethodsProperty;
        #endregion

        private static readonly Dictionary<Object, Editor> s_conditionEditors = new();
        private static readonly Dictionary<Object, Editor> s_scoringMethodEditors = new();

        private static bool s_conditionsFoldout = false;
        private static bool s_scoringMethodsFoldout = false;

        private void OnEnable()
        {
            m_scoreScalingProperty = serializedObject.FindProperty(s_scoreScalingPropertyName);
            m_displayNameProperty = serializedObject.FindProperty(s_displayNamePropertyName);
            m_actionTypeProperty = serializedObject.FindProperty(s_actionTypePropertyName);
            m_timelineAssetProperty = serializedObject.FindProperty(s_timelineAssetPropertyName);
            m_damageProperty = serializedObject.FindProperty(s_damagePropertyName);
            m_rangeProperty = serializedObject.FindProperty(s_rangePropertyName);
            m_statusesProperty = serializedObject.FindProperty(s_statusesPropertyName);
            m_conditionsProperty = serializedObject.FindProperty(s_conditionsPropertyName);
            m_scoringMethodsProperty = serializedObject.FindProperty(s_scoringMethodsPropertyName);
        }

        public override void OnInspectorGUI()
        {
            // Draw the normal properties
            EGL.PropertyField(m_scoreScalingProperty);
            EGL.PropertyField(m_displayNameProperty);
            EGL.PropertyField(m_actionTypeProperty);
            EGL.PropertyField(m_timelineAssetProperty);
            EGL.PropertyField(m_damageProperty);
            EGL.PropertyField(m_rangeProperty);
            EGL.PropertyField(m_statusesProperty);

            // Conditions property editor
            s_conditionsFoldout = EGL.Foldout(s_conditionsFoldout, "Conditions", EditorStyles.foldoutHeader);
            if (s_conditionsFoldout)
            {
                EGL.BeginVertical(GUI.skin.window);
                for (int i = 0, count = m_conditionsProperty.arraySize; i < count; i++)
                {
                    SerializedProperty conditionProperty = m_conditionsProperty.GetArrayElementAtIndex(i);
                    var objectValue = (SerializedCondition)conditionProperty.objectReferenceValue;

                    if (!s_conditionEditors.TryGetValue(objectValue, out Editor editor))
                    {
                        editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                        s_conditionEditors.Add(objectValue, editor);
                    }

                    EGL.BeginVertical(GUI.skin.box);

                    EGL.LabelField("Condition " + i, EditorStyles.boldLabel);
                    objectValue.name = EGL.TextField("Name", objectValue.name);

                    editor.OnInspectorGUI();

                    if (GUILayout.Button("Remove Condition"))
                    {
                        DestroyImmediate(objectValue, true);
                        SerializedPropertyHelper.CompletelyRemove(m_conditionsProperty, i);

                        --i;
                        count = m_conditionsProperty.arraySize;

                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    EGL.EndVertical();
                }

                if (GUILayout.Button("Add Condition"))
                {
                    AddCondition();
                }

                EGL.EndVertical();
            }

            EGL.Space();

            // Scoring Methods property editor
            s_scoringMethodsFoldout = EGL.Foldout(s_scoringMethodsFoldout, "Scoring Methods", EditorStyles.foldoutHeader);
            if (s_scoringMethodsFoldout)
            {
                EGL.BeginVertical(GUI.skin.window);

                for (int i = 0, count = m_scoringMethodsProperty.arraySize; i < count; i++)
                {
                    SerializedProperty scoringMethodProperty = m_scoringMethodsProperty.GetArrayElementAtIndex(i);
                    var objectValue = (SerializedScoringMethod_Base)scoringMethodProperty.objectReferenceValue;

                    if (!s_scoringMethodEditors.TryGetValue(objectValue, out Editor editor))
                    {
                        editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                        s_scoringMethodEditors.Add(objectValue, editor);
                    }

                    EGL.BeginVertical(GUI.skin.box);

                    EGL.LabelField("Scoring Method " + i, EditorStyles.boldLabel);
                    objectValue.name = EGL.TextField("Name", objectValue.name);

                    editor.OnInspectorGUI();

                    if (GUILayout.Button("Remove Scoring Method"))
                    {
                        DestroyImmediate(objectValue, true);
                        SerializedPropertyHelper.CompletelyRemove(m_scoringMethodsProperty, i);

                        --i;
                        count = m_scoringMethodsProperty.arraySize;

                        serializedObject.ApplyModifiedProperties();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    EGL.EndVertical();
                }

                EGL.Separator();

                if (GUILayout.Button("Add Scoring Method"))
                {
                    AddScoringMethod();
                }

                EGL.EndVertical();
            }
        }

        private void AddCondition()
        {
            ScriptableObject instance = CreateInstance<SerializedCondition>();
            instance.name = "New Serialized Condition";

            AssetDatabase.AddObjectToAsset(instance, target);

            int index = m_conditionsProperty.arraySize++;
            m_conditionsProperty.GetArrayElementAtIndex(index).objectReferenceValue = instance;

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        private void AddScoringMethod()
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> scoringMethodTypes = SerializedScoringMethodTypesCollection.scoringMethodTypes;

            for (int i = 0, count = scoringMethodTypes.Count; i < count; i++)
            {
                Type type = scoringMethodTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    Type serializedScoringMethodType = SerializedScoringMethodTypesCollection.GetSerializedScoringMethodType(type);
                    ScriptableObject instance = CreateInstance(serializedScoringMethodType);
                    instance.name = uiName.Replace(" ", string.Empty);

                    AssetDatabase.AddObjectToAsset(instance, target);

                    int index = m_scoringMethodsProperty.arraySize++;
                    m_scoringMethodsProperty.GetArrayElementAtIndex(index).objectReferenceValue = instance;

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                });
            }

            genericMenu.ShowAsContext();
        }

        public static string GetUIName(Type type)
        {
            return TypeHelper.GetDisplayName(type);
        }
    }
}
