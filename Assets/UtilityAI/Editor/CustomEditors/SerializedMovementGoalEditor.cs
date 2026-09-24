using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
{
    using Serialization;
    using EditorTools;

    [CustomEditor(typeof(SerializedMovementGoal_Base), true)]
    public class SerializedMovementGoalEditor : Editor
    {
        private const string s_scoreScalingPropertyName = "m_scoreScaling";
        private const string s_durationPropertyName = "m_duration";
        private const string s_displayNamePropertyName = "m_displayName";
        private const string s_conditionsPropertyName = "m_conditions";
        private const string s_scoringMethodsPropertyName = "m_scoringMethods";

        private static readonly string[] s_basePropertyNames = new string[]
        {
            s_scoreScalingPropertyName,
            s_durationPropertyName,
            s_displayNamePropertyName,
            s_conditionsPropertyName,
            s_scoringMethodsPropertyName
        };

        private SerializedProperty m_scoreScalingProperty;
        private SerializedProperty m_durationProperty;
        private SerializedProperty m_displayNameProperty;
        private SerializedProperty m_conditionsProperty;
        private SerializedProperty m_scoringMethodsProperty;

        private Type m_targetType;

        private static readonly Dictionary<Object, Editor> s_conditionEditors = new();
        private static readonly Dictionary<Object, Editor> s_scoringMethodEditors = new();

        private static bool s_conditionsFoldout;
        private static bool s_scoringMethodsFoldout;

        private void OnEnable()
        {
            m_scoreScalingProperty = serializedObject.FindProperty(s_scoreScalingPropertyName);
            m_durationProperty = serializedObject.FindProperty(s_durationPropertyName);
            m_displayNameProperty = serializedObject.FindProperty(s_displayNamePropertyName);
            m_conditionsProperty = serializedObject.FindProperty(s_conditionsPropertyName);
            m_scoringMethodsProperty = serializedObject.FindProperty(s_scoringMethodsPropertyName);

            m_targetType = target.GetType();
        }

        public override void OnInspectorGUI()
        {
            // Draw script field
            using (new EditorGUI.DisabledScope(true))
            {
                EGL.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            EditorGUI.BeginChangeCheck();

            // Draw base properties
            EGL.LabelField("Base Properties", EditorStyles.boldLabel);
            EGL.PropertyField(m_scoreScalingProperty);
            EGL.PropertyField(m_durationProperty);
            EGL.PropertyField(m_displayNameProperty);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            // Conditions property editor
            EGL.BeginHorizontal();
            s_conditionsFoldout = EGL.Foldout(s_conditionsFoldout, "Conditions", EditorStyles.foldoutHeader);
            using (new EditorGUI.DisabledScope(true))
            {
                EGL.IntField(GUIContent.none, m_conditionsProperty.arraySize, GUILayout.MaxWidth(48f));
            }
            EGL.EndHorizontal();
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

            // Scoring Methods property editor
            EGL.BeginHorizontal();
            s_scoringMethodsFoldout = EGL.Foldout(s_scoringMethodsFoldout, "Scoring Methods", EditorStyles.foldoutHeader);
            using (new EditorGUI.DisabledScope(true))
            {
                EGL.IntField(GUIContent.none, m_scoringMethodsProperty.arraySize, GUILayout.MaxWidth(48f));
            }
            EGL.EndHorizontal();
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

            EGL.Separator();

            // Draw any Additional properties
            string typeName = GetUIName(m_targetType);
            EGL.LabelField(new GUIContent(typeName + " Properties"), EditorStyles.boldLabel);

            // Move to the first visible property
            EditorGUI.BeginChangeCheck();
            SerializedProperty prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    // Skip the script reference and the properties we have already drawn
                    if (prop.name == "m_Script" || s_basePropertyNames.Contains(prop.name))
                        continue;

                    // This draws everything else
                    EGL.PropertyField(prop, true);
                }
                while (prop.NextVisible(false)); // Use 'false' to avoid drawing child elements of complex structs/arrays twice
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void AddCondition()
        {
            ScriptableObject instance = CreateInstance<SerializedCondition>();
            instance.name = "New Condition";

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
            string typeName = type.Name;
            if (typeName[..10] == "Serialized")
                return Regex.Replace(type.Name[10..], "(\\B[A-Z])", " $1");
            return Regex.Replace(type.Name, "(\\B[A-Z])", " $1");
        }
    }
}
