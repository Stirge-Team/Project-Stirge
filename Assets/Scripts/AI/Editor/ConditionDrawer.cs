using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    [CustomPropertyDrawer(typeof(Condition))]
    public class ConditionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeIndex = property.FindPropertyRelative("m_typeIndex");
            int currentType = typeIndex.intValue;

            label = new GUIContent(StateEditor.StringTypes[currentType]);
            EditorGUI.BeginProperty(position, label, property);
            Rect rectFoldout = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);
            if (property.isExpanded)
            {
                Rect changeTypePopup = new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight, position.size.x - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                int popupValue = EditorGUI.Popup(changeTypePopup, currentType, StateEditor.StringTypes);
                if (popupValue != currentType)
                {
                    typeIndex.intValue = popupValue;
                }

                Rect rectInvertProp = new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * 2, position.size.x - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                SerializedProperty invertValueProp = property.FindPropertyRelative("m_invertValue");
                EditorGUI.PropertyField(rectInvertProp, invertValueProp);

                if (StateEditor.ConditionTypes[currentType].Name == nameof(DistanceCondition))
                {
                    Rect rectDistance = new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * 3, position.size.x - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                    SerializedProperty distanceProp = property.FindPropertyRelative("m_distance");
                    EditorGUI.PropertyField(rectDistance, distanceProp);
                }
            }

            /*
            int currentType = property.FindPropertyRelative("m_typeIndex").intValue;
            EditorGUI.LabelField(position, StateEditor.StringTypes[currentType]);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("m_invertValue"));

            */
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (property.isExpanded)
            {
                totalLines += 2; // for type popup and invert value prop
                int currentType = property.FindPropertyRelative("m_typeIndex").intValue;
                switch (StateEditor.ConditionTypes[currentType].Name)
                {
                    case nameof(DistanceCondition):
                        totalLines++; // for distance prop
                        break;
                    default:
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
