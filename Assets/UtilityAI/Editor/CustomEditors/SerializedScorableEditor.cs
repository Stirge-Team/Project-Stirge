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
    using EditorTools;
    using Serialization;

    public abstract class SerializedScorableEditor<T> : Editor where T : class, IScorable
    {
        protected abstract string[] basePropertyNames { get; }
        protected abstract SerializedProperty conditionsProperty { get; }
        protected abstract SerializedProperty scoringMethodsProperty { get; }

        private static readonly Dictionary<Object, Editor> s_conditionEditors = new();
        private static readonly Dictionary<Object, Editor> s_scoringMethodEditors = new();

        private static bool s_conditionsFoldout = false;
        private static bool s_scoringMethodsFoldout = false;

        private Type m_targetType;

        private void OnEnable()
        {
            m_targetType = target.GetType();
            FindSerializedProperties();
        }
        protected abstract void FindSerializedProperties();

        public sealed override void OnInspectorGUI()
        {
            // Draw script field
            using (new EditorGUI.DisabledScope(true))
            {
                EGL.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            EditorGUI.BeginChangeCheck();

            EGL.LabelField("Base Properties", EditorStyles.boldLabel);
            DrawBaseProperties();

            // Conditions property editor
            DrawConditionsProperty();

            // Scoring Methods property editor
            DrawScoringMethodsProperty();

            EGL.Separator();

            DrawAdditionalProperties();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        protected abstract void DrawBaseProperties();

        private void DrawConditionsProperty()
        {
            EGL.BeginHorizontal();
            s_conditionsFoldout = EGL.Foldout(s_conditionsFoldout, "Conditions", EditorStyles.foldoutHeader);
            using (new EditorGUI.DisabledScope(true))
            {
                EGL.IntField(GUIContent.none, conditionsProperty.arraySize, GUILayout.MaxWidth(48f));
            }
            EGL.EndHorizontal();
            if (s_conditionsFoldout)
            {
                EGL.BeginVertical(GUI.skin.window);
                for (int i = 0, count = conditionsProperty.arraySize; i < count; i++)
                {
                    SerializedProperty conditionProperty = conditionsProperty.GetArrayElementAtIndex(i);
                    var objectValue = (SerializedCondition_Base)conditionProperty.objectReferenceValue;

                    if (!s_conditionEditors.TryGetValue(objectValue, out Editor editor))
                    {
                        editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                        s_conditionEditors.Add(objectValue, editor);
                    }

                    EGL.BeginVertical(GUI.skin.box);

                    EGL.LabelField("Condition " + i, EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    objectValue.name = EGL.TextField("Name", objectValue.name);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }

                    editor.OnInspectorGUI();

                    if (GUILayout.Button("Remove Condition"))
                    {
                        DestroyImmediate(objectValue, true);
                        SerializedPropertyHelper.CompletelyRemove(conditionsProperty, i);

                        --i;
                        count = conditionsProperty.arraySize;

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
        }

        private void DrawScoringMethodsProperty()
        {
            EGL.BeginHorizontal();
            s_scoringMethodsFoldout = EGL.Foldout(s_scoringMethodsFoldout, "Scoring Methods", EditorStyles.foldoutHeader);
            using (new EditorGUI.DisabledScope(true))
            {
                EGL.IntField(GUIContent.none, scoringMethodsProperty.arraySize, GUILayout.MaxWidth(48f));
            }
            EGL.EndHorizontal();
            if (s_scoringMethodsFoldout)
            {
                EGL.BeginVertical(GUI.skin.window);

                for (int i = 0, count = scoringMethodsProperty.arraySize; i < count; i++)
                {
                    SerializedProperty scoringMethodProperty = scoringMethodsProperty.GetArrayElementAtIndex(i);
                    var objectValue = (SerializedScoringMethod_Base)scoringMethodProperty.objectReferenceValue;

                    if (!s_scoringMethodEditors.TryGetValue(objectValue, out Editor editor))
                    {
                        editor = CreateEditorWithContext(new Object[] { objectValue }, target);
                        s_scoringMethodEditors.Add(objectValue, editor);
                    }

                    EGL.BeginVertical(GUI.skin.box);

                    EGL.LabelField("Scoring Method " + i, EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    objectValue.name = EGL.TextField("Name", objectValue.name);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();

                    editor.OnInspectorGUI();

                    if (GUILayout.Button("Remove Scoring Method"))
                    {
                        DestroyImmediate(objectValue, true);
                        SerializedPropertyHelper.CompletelyRemove(scoringMethodsProperty, i);

                        --i;
                        count = scoringMethodsProperty.arraySize;

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

        private void DrawAdditionalProperties()
        {
            // Draw any Additional properties, unless its the non-generic Action type which has no more properties
            string typeName = GetUIName(m_targetType);
            if (typeName == GetUIName(typeof(Action)))
                return;

            EGL.LabelField(new GUIContent(typeName + " Properties"), EditorStyles.boldLabel);

            // Move to the first visible property
            EditorGUI.BeginChangeCheck();
            SerializedProperty prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    // Skip the script reference and the properties we have already drawn
                    if (prop.name == "m_Script" || basePropertyNames.Contains(prop.name))
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
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> conditionTypes = SerializedConditionTypesCollection.GetScoringMethodTypes<T>();

            for (int i = 0, count = conditionTypes.Count; i < count; i++)
            {
                Type type = conditionTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    Type serializedConditionType = SerializedConditionTypesCollection.GetSerializedConditionType<T>(type);
                    ScriptableObject instance = CreateInstance(serializedConditionType);
                    instance.name = instance.name = uiName.Replace(" ", string.Empty);

                    AssetDatabase.AddObjectToAsset(instance, target);

                    int index = conditionsProperty.arraySize++;
                    SerializedProperty newConditionProperty = conditionsProperty.GetArrayElementAtIndex(index);
                    newConditionProperty.objectReferenceValue = instance;
                    if (newConditionProperty.objectReferenceValue is SerializedGenericCondition genericCondition)
                    {
                        genericCondition.SetScorableType(GetScorableType());
                    }

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                });
            }

            genericMenu.ShowAsContext();
        }

        private void AddScoringMethod()
        {
            var genericMenu = new GenericMenu();
            IReadOnlyList<Type> scoringMethodTypes = SerializedScoringMethodTypesCollection.GetScoringMethodTypes<T>();

            for (int i = 0, count = scoringMethodTypes.Count; i < count; i++)
            {
                Type type = scoringMethodTypes[i];
                string uiName = GetUIName(type);
                genericMenu.AddItem(new GUIContent(uiName), false, () =>
                {
                    Type serializedScoringMethodType = SerializedScoringMethodTypesCollection.GetSerializedScoringMethodType<T>(type);
                    ScriptableObject instance = CreateInstance(serializedScoringMethodType);
                    instance.name = uiName.Replace(" ", string.Empty);

                    AssetDatabase.AddObjectToAsset(instance, target);

                    int index = scoringMethodsProperty.arraySize++;
                    scoringMethodsProperty.GetArrayElementAtIndex(index).objectReferenceValue = instance;

                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                });
            }

            genericMenu.ShowAsContext();
        }

        private static SerializedScorableType GetScorableType()
        {
            return typeof(T).Name switch
            {
                nameof(Action) => SerializedScorableType.Action,
                nameof(MovementGoal) => SerializedScorableType.MovementGoal,
                nameof(Status) => SerializedScorableType.MovementGoal,
                _ => throw new Exception(typeof(SerializedScorableEditor<>).Name + " cannot use " + typeof(IScorable).Name + " as its generic argument."),
            };
        }

        private static string GetUIName(Type type)
        {
            string typeName = type.Name;
            int indexOfUnderscore = typeName.IndexOf('_');
            if (indexOfUnderscore > 0)
                typeName = typeName[..indexOfUnderscore];

            if (typeName.Length >= 10 && typeName[..10] == "Serialized")
                typeName = Regex.Replace(typeName[10..], "(\\B[A-Z])", " $1");
            else
                typeName = Regex.Replace(typeName, "(\\B[A-Z])", " $1");

            return typeName;
        }
    }
}
