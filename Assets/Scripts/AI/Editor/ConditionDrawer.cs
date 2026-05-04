using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    [CustomPropertyDrawer(typeof(Condition))]
    public class ConditionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int totalLines = 0;

            void DrawPropertyField(string propertyName)
            {
                SerializedProperty propToDraw = property.FindPropertyRelative(propertyName);
                Rect propRect = GetNewRect();
                EditorGUI.PropertyField(propRect, propToDraw);

                if (propToDraw.propertyType == SerializedPropertyType.Float)
                {
                    if (propToDraw.floatValue < 0)
                        propToDraw.floatValue = 0;
                }
                if (propToDraw.isArray && propToDraw.isExpanded)
                {
                    totalLines += (int)(EditorGUI.GetPropertyHeight(propToDraw) / EditorGUIUtility.singleLineHeight);
                }
            }

            Rect GetNewRect()
            {
                totalLines++;
                return new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * (totalLines - 1), position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
            }

            // get the string TypeName of the Condition
            string typeName = property.managedReferenceFullTypename;

            if (typeName.Length < 26)
            {
                label.text = "Empty, pls delete";
            }
            else
            {
                typeName = typeName[26..];
                label.text = typeName[..^9];

                // add "Not" to the start of a label if the value is inverted
                if (property.FindPropertyRelative("m_invertValue").boolValue)
                {
                    label.text = "Not " + label.text;
                }
            }
            
            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(GetNewRect(), property.isExpanded, label);
            if (property.isExpanded && label.text != "Empty, pls delete")
            {
                // draw the Invert Value prop
                DrawPropertyField("m_invertValue");

                if (typeName == nameof(DistanceCondition))
                    DrawPropertyField("m_distance");
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1; // for foldout

            string typeName = property.managedReferenceFullTypename;
            if (typeName.Length < 26)
                typeName = "";
            else
                typeName = typeName[26..];
            
            if (property.isExpanded && typeName != "")
            {
                totalLines++; // for invert value prop

                switch (typeName)
                {
                    case nameof(DistanceCondition):
                        totalLines++;
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
