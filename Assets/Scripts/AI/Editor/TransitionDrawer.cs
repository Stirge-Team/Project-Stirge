using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    [CustomPropertyDrawer(typeof(Transition))]
    public class TransitionDrawer : PropertyDrawer
    {
        private int m_selectedCondition;

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

            SerializedProperty targetStateProp = property.FindPropertyRelative("m_targetState");

            if (targetStateProp.objectReferenceValue != null)
            {
                label = new GUIContent("Transition to " + targetStateProp.objectReferenceValue.name);
            }
            else
            {
                label = new GUIContent("Empty Transition");
            }

            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(GetNewRect(), property.isExpanded, label);
            if (property.isExpanded)
            {
                // draw the target state for the Transition
                DrawPropertyField("m_targetState");

                // draw conditions list
                DrawPropertyField("m_conditions");

                // draw popup for adding new Conditions
                m_selectedCondition = EditorGUI.Popup(GetNewRect(), m_selectedCondition, StateEditor.ConditionNames);

                // create new Condition button
                if (GUI.Button(GetNewRect(), new GUIContent("Add new " + StateEditor.ConditionNames[m_selectedCondition])))
                {
                    SerializedProperty conditionsProp = property.FindPropertyRelative("m_conditions");
                    Condition newCondition = System.Activator.CreateInstance(Condition.ConditionTypes[m_selectedCondition]) as Condition;
                    conditionsProp.arraySize++;
                    SerializedProperty newConditionProp = conditionsProp.GetArrayElementAtIndex(conditionsProp.arraySize - 1);
                    newConditionProp.managedReferenceValue = newCondition;
                    newConditionProp.isExpanded = true;
                    conditionsProp.isExpanded = true;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (property.isExpanded)
            {
                totalLines += 4; // for target state, new Condition popup, new Conditio button, and size of Conditions array prop
                SerializedProperty conditionsProp = property.FindPropertyRelative("m_conditions");
                if (conditionsProp.isExpanded)
                    totalLines += (int)(EditorGUI.GetPropertyHeight(conditionsProp) / EditorGUIUtility.singleLineHeight);
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
