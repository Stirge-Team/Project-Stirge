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

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SaveProperties(property);
            return GetHeight(label);
        }

        protected abstract float GetHeight(GUIContent label);

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

        protected void DrawPropertyField(string propertyName)
        {
            SerializedProperty propToDraw = FindPropertyRelative(propertyName);
            if (propToDraw != null)
            {
                EditorGUI.PropertyField(GetNewRect(), propToDraw);
                if (propToDraw.propertyType == SerializedPropertyType.Float)
                {
                    if (propToDraw.floatValue < 0)
                        propToDraw.floatValue = 0;
                }
                else if (propToDraw.isArray && propToDraw.isExpanded)
                {
                    m_totalLines += GetPropertyLineHeight(propToDraw);
                }
                else if (propToDraw.type == nameof(RandomFloatField))
                {
                    m_totalLines += GetPropertyLineHeight(propToDraw) - 1;
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
            return new Rect(m_position.min.x + EditorGUI.indentLevel * 15f, m_position.min.y + EditorGUIUtility.singleLineHeight * (m_totalLines - 1), m_position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
        }

        protected SerializedProperty FindPropertyRelative(string propertyName)
        {
            if (!m_cachedProperties.ContainsKey(propertyName))
            {
                SerializedProperty property = m_property.FindPropertyRelative(propertyName);
                if (property == null)
                {
                    Debug.LogWarning("Could not find property relative with name '" + propertyName + "', path '" + m_property.propertyPath + '.');
                    return null;
                }

                m_cachedProperties.Add(propertyName, property);
            }

            return m_cachedProperties[propertyName];
        }

        protected int GetPropertyLineHeight(string propertyName)
        {
            SerializedProperty property = FindPropertyRelative(propertyName);
            return GetPropertyLineHeight(property);
        }

        protected int GetPropertyLineHeight(SerializedProperty property)
        {
            if (property != null)
            {
                int lines = (int)(EditorGUI.GetPropertyHeight(property) / EditorGUIUtility.singleLineHeight);
                if (property.isExpanded)
                {
                    if (property.isArray)
                    {
                        lines++; // for +/- button
                        if (property.arraySize < 2)
                            lines--;
                    }
                }
                return lines;
            }
            else
                return 1;
        }

        protected void SetLabelTextToTypeName(GUIContent label)
        {
            if (m_property != null)
                label.text = m_property.managedReferenceValue.GetType().Name;
            else
                label.text = "Empty, pls delete";
        }

        protected void DrawLabelHeader(GUIContent label)
        {
            // If property is part of an array
            if (PropertyIsArrayElement())
            {
                m_property.isExpanded = EditorGUI.Foldout(GetNewRect(), m_property.isExpanded, label);
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