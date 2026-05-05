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
            Rect propRect = GetNewRect();
            EditorGUI.PropertyField(propRect, propToDraw);

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

        protected Rect GetNewRect()
        {
            m_totalLines++;
            return new Rect(m_position.min.x + EditorGUI.indentLevel * 15f, m_position.min.y + EditorGUIUtility.singleLineHeight * (m_totalLines - 1), m_position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
        }

        protected SerializedProperty FindPropertyRelative(string propertyName)
        {
            if (!m_cachedProperties.ContainsKey(propertyName))
            { 
                m_cachedProperties.Add(propertyName, m_property.FindPropertyRelative(propertyName));
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
            int lines = (int)(EditorGUI.GetPropertyHeight(property) / EditorGUIUtility.singleLineHeight);
            if (property.isArray && property.isExpanded)
                lines++; // for +/- button
            return lines;
        }
    }
}
