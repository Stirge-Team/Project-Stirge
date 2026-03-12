using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    [CustomPropertyDrawer(typeof(Transition))]
    public class TransitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
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
            Rect rectFoldout = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);
            if (property.isExpanded)
            {
                Rect targetStateRect = new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight, position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(targetStateRect, targetStateProp);

                Rect conditionsRect = new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * 2, position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
                SerializedProperty conditionsProp = property.FindPropertyRelative("m_conditions");
                if (conditionsProp != null)
                {
                    int[] oldTypes = new int[conditionsProp.arraySize];
                    int oldArraySize = conditionsProp.arraySize;

                    // populate old types and also check for null elements
                    for (int i = 0; i < oldArraySize; i++)
                    {
                        var condition = conditionsProp.GetArrayElementAtIndex(i);
                        condition.managedReferenceValue ??= new Condition();
                        oldTypes[i] = conditionsProp.GetArrayElementAtIndex(i).FindPropertyRelative("m_typeIndex").intValue;
                    }
                    
                    EditorGUI.PropertyField(conditionsRect, conditionsProp);

                    if (oldArraySize == conditionsProp.arraySize)
                    {
                        for (int i = 0; i < conditionsProp.arraySize; i++)
                        {
                            SerializedProperty condition = conditionsProp.GetArrayElementAtIndex(i);
                            int newType = condition.FindPropertyRelative("m_typeIndex").intValue;
                            if (newType != oldTypes[i])
                            {
                                Condition newCondition = System.Activator.CreateInstance(StateEditor.ConditionTypes[newType]) as Condition;
                                condition.managedReferenceValue = newCondition;
                                condition.FindPropertyRelative("m_typeIndex").intValue = newType;
                            }
                        }
                    }
                }
                else
                {
                    conditionsProp.managedReferenceValue = new Condition[0];
                    EditorGUI.BeginDisabledGroup(true);
                    Rect noConditionsTextRect = new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * 3, position.size.x - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.TextField(noConditionsTextRect, "No Conditions!");
                    EditorGUI.EndDisabledGroup();
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
                totalLines++; // target state prop
                SerializedProperty conditionsProp = property.FindPropertyRelative("m_conditions");
                totalLines += (int)(EditorGUI.GetPropertyHeight(conditionsProp) / EditorGUIUtility.singleLineHeight);
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
