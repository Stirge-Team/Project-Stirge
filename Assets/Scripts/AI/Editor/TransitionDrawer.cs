using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    using Tools;

    [CustomPropertyDrawer(typeof(Transition))]
    public class TransitionDrawer : EasyPropertyDrawer
    {
        private int m_selectedCondition;

        protected override void DrawGUI(GUIContent label)
        {
            SerializedProperty targetStateProp = FindPropertyRelative("m_targetState");

            if (targetStateProp.objectReferenceValue != null)
            {
                label.text = "Transition to " + targetStateProp.objectReferenceValue.name;
            }
            else
            {
                label.text = "Empty Transition";
            }

            EditorGUI.BeginProperty(m_position, label, m_property);
            DrawLabelHeader(label);
            if (m_property.isExpanded)
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
                    SerializedProperty conditionsProp = FindPropertyRelative("m_conditions");
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

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (m_property.isExpanded)
            {
                totalLines += 3; // for target state, new Condition popup, and new Condition button
                totalLines += GetPropertyLineHeight("m_conditions"); // for Conditions array
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
