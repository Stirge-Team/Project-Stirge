using UnityEditor;
using UnityEngine;

namespace Stirge.Tools
{
    [CustomPropertyDrawer(typeof(RandomVector2Field))]
    public class RandomVector2FieldDrawer : EasyPropertyDrawer
    {
        protected override void DrawGUI(GUIContent label)
        {
            // If property is not expanded, show the value in the label
            if (!m_property.isExpanded)
            {
                string newText = ": ";

                SerializedProperty xProp = FindPropertyRelative("m_x");
                SerializedProperty yProp = FindPropertyRelative("m_y");

                bool xRandom = xProp.FindPropertyRelative("m_isRandom").boolValue;
                bool yRandom = yProp.FindPropertyRelative("m_isRandom").boolValue;

                if (xRandom)
                {
                    Vector2 range = xProp.FindPropertyRelative("m_range").vector2Value;
                    newText += $"{{x: {range.x} - {range.y}, ";
                }
                else
                {
                    newText += "{x: " + xProp.FindPropertyRelative("m_value").floatValue + ", ";
                }
                if (yRandom)
                {
                    Vector2 range = yProp.FindPropertyRelative("m_range").vector2Value;
                    newText += $"y: {range.x} - {range.y}}}";
                }
                else
                {
                    newText += "y: " + yProp.FindPropertyRelative("m_value").floatValue + "}";
                }

                label.text += newText;
            }

            EditorGUI.BeginProperty(m_position, label, m_property);
            m_property.isExpanded = EditorGUI.Foldout(GetNewRect(), m_property.isExpanded, label);
            if (m_property.isExpanded)
            {
                // if not part of an array, indent
                bool isInArray = m_property.propertyPath[^1] == ']';
                if (!isInArray)
                    EditorGUI.indentLevel++;

                DrawPropertyField("m_x");
                DrawPropertyField("m_y");

                if (!isInArray)
                    EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1;
            if (m_property.isExpanded)
            {
                totalLines += GetPropertyLineHeight("m_x");
                totalLines += GetPropertyLineHeight("m_y");
            }
            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1); ;
        }
    }
}
