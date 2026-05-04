using UnityEditor;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    [CustomPropertyDrawer(typeof(AttackNode), true)]
    public class AttackNodeDrawer : PropertyDrawer
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

            string typeName = property.type;
            typeName = typeName.Substring(17, typeName.Length - 18);

            Debug.Log(typeName);

            // if AttackNode, then prompt deletion
            if (typeName == string.Empty)
                label.text = "Empty, pls delete";
            else
                label.text = typeName;
        }
    }
}
