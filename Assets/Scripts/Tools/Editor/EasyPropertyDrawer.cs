using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stirge.Tools
{
    public abstract class EasyPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, SerializedProperty> m_cachedProperties = new();

        protected int m_totalLines;
        protected SerializedProperty m_property;
        protected Rect m_position;

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_totalLines = 0;
            SaveProperties(property, position);

            DrawGUI(label);
        }

        protected abstract void DrawGUI(GUIContent label);

        /// <summary>
        /// Do not use this to get the height of an <see cref="EasyPropertyDrawer"/> property!! Instead use <see cref="GetPropertyLineHeight(SerializedProperty)"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SaveProperties(property);
            int totalLines = GetHeight(label);
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }

        protected abstract int GetHeight(GUIContent label);

        protected void SaveProperties(SerializedProperty property)
        {
            m_property = property;
            m_cachedProperties.Clear();
        }
        protected void SaveProperties(SerializedProperty property, Rect position)
        {
            m_property = property;
            m_position = position;
            m_cachedProperties.Clear();
        }

        protected void DrawPropertyField(string propertyName, GUIContent label = null)
        {
            SerializedProperty propertyToDraw = FindPropertyRelative(propertyName);
            if (propertyToDraw != null)
            {
                if (label == null)
                    EditorGUI.PropertyField(GetNewRect(), propertyToDraw);
                else
                    EditorGUI.PropertyField(GetNewRect(), propertyToDraw, label);

                if (propertyToDraw.propertyType == SerializedPropertyType.Float)
                {
                    if (propertyToDraw.floatValue < 0)
                        propertyToDraw.floatValue = 0;
                }
                else if (propertyToDraw.isArray && propertyToDraw.isExpanded)
                {
                    m_totalLines += GetPropertyLineHeight(propertyToDraw);
                }
                else if (propertyToDraw.type == nameof(RandomFloatField))
                {
                    m_totalLines += GetPropertyLineHeight(propertyToDraw) - 1;
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.TextArea(GetNewRect(), $"Problem drawing '{propertyName}' property, path '{m_property.propertyPath}'.");
                EditorGUI.EndDisabledGroup();
            }
        }

        protected Rect GetNewRect()
        {
            m_totalLines++;
            return new Rect(
                /* x */ m_position.min.x + EditorGUI.indentLevel * 15f,
                /* y */ m_position.min.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (m_totalLines - 1),
                /* w */ m_position.size.x - EditorGUI.indentLevel * 15f,
                /* h */ EditorGUIUtility.singleLineHeight);
        }

        protected SerializedProperty FindPropertyRelative(string propertyName)
        {
            if (!m_cachedProperties.TryGetValue(propertyName, out var property))//!m_cachedProperties.ContainsKey(propertyName))
            {
                property = m_property.FindPropertyRelative(propertyName);
                if (property == null)
                {
                    Debug.LogWarning($"Could not find property relative with Name '{propertyName}' with Path '{m_property.propertyPath}'.");
                    return null;
                }

                m_cachedProperties.Add(propertyName, property);
            }

            return property;
        }

        protected int GetPropertyLineHeight(string propertyName)
        {
            SerializedProperty property = FindPropertyRelative(propertyName);
            return GetPropertyLineHeight(property);
        }

        protected static int GetPropertyLineHeight(SerializedProperty property)
        {
            if (property != null)
            {
                int lines = (int)(EditorGUI.GetPropertyHeight(property) / EditorGUIUtility.singleLineHeight);
                if (property.isExpanded && property.isArray)
                {
                    if (property.arraySize < 2)
                        lines++; // for +/- button
                }
                return lines;
            }
            else
                return 1;
        }

        protected void SetLabelTextToTypeName(GUIContent label)
        {
            if (m_property != null && m_property.managedReferenceValue != null)
                label.text = m_property.managedReferenceValue.GetType().Name;
            else
                label.text = "Empty, pls delete";
        }

        protected void DrawLabelHeader(GUIContent label)
        {
            // If property is part of an array
            if (PropertyIsArrayElement())
            {
                m_property.isExpanded = EditorGUI.Foldout(GetNewRect(), m_property.isExpanded, label, EditorStyles.foldout);
            }
            else
            {
                EditorGUI.LabelField(GetNewRect(), label, EditorStyles.boldLabel);
                m_property.isExpanded = true;
            }
        }
        
        protected bool PropertyIsArrayElement()
        {
            return m_property.propertyPath.EndsWith(']');
        }
    }

}