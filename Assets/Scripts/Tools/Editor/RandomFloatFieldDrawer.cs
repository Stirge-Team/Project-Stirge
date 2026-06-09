using UnityEngine;
using UnityEditor;

namespace Stirge.Tools
{
    [CustomPropertyDrawer(typeof(RandomFloatField))]
    public class RandomFloatFieldDrawer : EasyPropertyDrawer
    {
        protected override void DrawGUI(GUIContent label)
        {
            // If property is not expanded, show the value
            if (!m_property.isExpanded)
            {
                if (FindPropertyRelative("m_isRandom").boolValue)
                {
                    SerializedProperty rangeProp = FindPropertyRelative("m_range");
                    label.text += $": {rangeProp.vector2Value.x} - {rangeProp.vector2Value.y}";
                }
                else
                {
                    label.text += $": {FindPropertyRelative("m_value").floatValue}";
                }
            }

            EditorGUI.BeginProperty(m_position, label, m_property);
            m_property.isExpanded = EditorGUI.Foldout(GetNewRect(), m_property.isExpanded, label);
            if (m_property.isExpanded)
            {
                // if not part of an array, indent
                bool isInArray = m_property.propertyPath[^1] == ']';
                if (!isInArray)
                    EditorGUI.indentLevel++;
                
                DrawPropertyField("m_isRandom");

                if (FindPropertyRelative("m_isRandom").boolValue)
                {
                    DrawPropertyField("m_range");
                }
                else
                {
                    DrawPropertyField("m_value");
                }

                if (!isInArray)
                    EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
        
        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (m_property.isExpanded)
            {
                totalLines += 2; // for m_isRandom bool and either m_value or m_range
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
