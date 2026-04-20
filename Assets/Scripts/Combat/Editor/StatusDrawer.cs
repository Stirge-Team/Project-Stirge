using UnityEditor;
using UnityEngine;

namespace Stirge.Combat
{
    [CustomPropertyDrawer(typeof(Status), true)]
    public class StatusDrawer : PropertyDrawer
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
            }

            Rect GetNewRect()
            {
                totalLines++;
                return new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * (totalLines - 1), position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
            }

            string typeName = property.managedReferenceFullTypename;
            if (typeName.Length < 30)
                label.text = "Empty, pls delete";
            else
                label.text = typeName[30..];

            EditorGUI.BeginProperty(position, label, property);
            Rect foldoutRect = GetNewRect();
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
            property.isExpanded = true;
            if (property.isExpanded)
            {
                switch (label.text)
                {
                    case nameof(AirJuggle):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_airStallLength");
                        DrawPropertyField("m_stunLength");
                        break;
                    case nameof(Knockback):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_height");
                        DrawPropertyField("m_stunLength");
                        break;
                    case nameof(Stun):
                        DrawPropertyField("m_stunLength");
                        break;
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (property.isExpanded)
            {
                string typeName = property.managedReferenceFullTypename;
                if (typeName.Length < 30)
                    typeName = string.Empty;
                else
                    typeName = typeName[30..];

                switch (typeName)
                {
                    case nameof(AirJuggle):
                    case nameof(Knockback):
                        totalLines += 3;
                        break;
                    case nameof(Stun):
                        totalLines++;
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
