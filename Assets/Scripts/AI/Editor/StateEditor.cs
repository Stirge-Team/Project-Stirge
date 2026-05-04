// https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-HowTo-CreateCustomInspector.html

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

using EG = UnityEditor.EditorGUI;
using EGL = UnityEditor.EditorGUILayout;
using GL = UnityEngine.GUILayout;

namespace Stirge.AI
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        #region Names
        public static string[] BehaviourNames
        {
            get
            {
                if (behaviourNames == null || behaviourNames.Length == 0)
                {
                    PopulateBehaviourNames();
                }
                return behaviourNames;
            }
        }
        // to display list of Condition names
        public static string[] ConditionNames
        {
            get
            {
                if (conditionNames == null || conditionNames.Length == 0)
                {
                    PopulateConditionNames();
                }
                return conditionNames;
            }
        }
        private static string[] behaviourNames;
        private static string[] conditionNames;
        #endregion

        private int m_selectedBehaviour;

        private bool m_hasChanged = false;

        private void OnEnable()
        {
            PopulateBehaviourNames();
            PopulateConditionNames();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EG.BeginChangeCheck();

            // show Script fields for quick editing
            using (new EG.DisabledScope(true))
            {
                EGL.ObjectField("Script", MonoScript.FromScriptableObject((State)target), typeof(State), false);
                EGL.ObjectField("Editor", MonoScript.FromScriptableObject(this), typeof(StateEditor), false);
            }

            OnGUI();

            if (EG.EndChangeCheck() || m_hasChanged)
            {
                m_hasChanged = false;
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnGUI()
        {
            // behaviours
            SerializedProperty behavioursProp = serializedObject.FindProperty("m_behaviours");
            EGL.PropertyField(behavioursProp);

            // draw popup for adding new Behaviours
            m_selectedBehaviour = EGL.Popup(m_selectedBehaviour, BehaviourNames);

            // create new Behaviour button
            if (GL.Button("Add new " + BehaviourNames[m_selectedBehaviour]))
            {
                Behaviour newBehaviour = System.Activator.CreateInstance(Behaviour.BehaviourTypes[m_selectedBehaviour]) as Behaviour;
                behavioursProp.arraySize++;
                SerializedProperty newBehaviourProp = behavioursProp.GetArrayElementAtIndex(behavioursProp.arraySize - 1);
                newBehaviourProp.managedReferenceValue = newBehaviour;
                newBehaviourProp.isExpanded = true;
                behavioursProp.isExpanded = true;
            }

            // draw Timed Transition properties
            SerializedProperty timedTransition = serializedObject.FindProperty("m_timedTransitionState");
            SerializedProperty timedTransitionDelay = serializedObject.FindProperty("m_timedTransitionDelay");
            EGL.LabelField("Timed Transition", EditorStyles.boldLabel);
            EG.indentLevel++;
            // show label saying to leave blank if no timed transition wanted
            EG.BeginDisabledGroup(true);
            EGL.TextField("Leave blank for no Timed Transition.");
            EG.EndDisabledGroup();
            EGL.PropertyField(timedTransition, new GUIContent("Target State"));
            EGL.PropertyField(timedTransitionDelay, new GUIContent("Delay", "Time in seconds from entering this State that a Transition will ooccur to the Timed Transition State."));
            EG.indentLevel--;

            // empty line
            EGL.Space(12f);

            // draw Transitions
            SerializedProperty transitions = serializedObject.FindProperty("m_transitions");
            EGL.PropertyField(transitions);

            /*
            // add and subtract buttons
            EGL.BeginHorizontal();
            if (GL.Button("Add Transition"))
            {
                transitions.InsertArrayElementAtIndex(transitions.arraySize);
                m_hasChanged = true;
            }
            if (transitions != null && transitions.arraySize > 0 && GL.Button("Remove Transition"))
            {
                transitions.DeleteArrayElementAtIndex(transitions.arraySize - 1);
                m_hasChanged = true;
            }
            EGL.EndHorizontal();
            */

        }

        private static void PopulateBehaviourNames()
        {
            // get all the Conditions from the static list and change them from CamelCase to English
            behaviourNames = Behaviour.BehaviourTypes.Select(t => t.Name).ToArray();
            for (int i = 0; i < behaviourNames.Length; i++)
            {
                // turns camel case into separate words (looks nice)
                behaviourNames[i] = Regex.Replace(Regex.Replace(behaviourNames[i], @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
        }
        private static void PopulateConditionNames()
        {
            // get all the Conditions from the static list and change them from CamelCase to English
            conditionNames = Condition.ConditionTypes.Select(t => t.Name).ToArray();
            for (int i = 0; i < conditionNames.Length; i++)
            {
                // turns camel case into separate words (looks nice)
                conditionNames[i] = Regex.Replace(Regex.Replace(conditionNames[i], @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
        }

        /*
        [System.Obsolete]
        private void DrawTransition(SerializedProperty transition)
        {
            // Target State field
            SerializedProperty targetState = transition.FindPropertyRelative("m_targetState");

            string labelText;
            if (targetState.objectReferenceValue != null)
            {
                labelText = "Transition to " + targetState.objectReferenceValue.name;
            }
            else
            {
                labelText = "Empty Transition";
            }
            EGL.LabelField(labelText, EditorStyles.boldLabel);

            EG.indentLevel++;
            EGL.PropertyField(targetState, new GUIContent("Target State"));

            // draw all Conditions
            SerializedProperty conditions = transition.FindPropertyRelative("m_conditions");
            //EGL.LabelField(new GUIContent("Conditions"), EditorStyles.boldLabel);
            if (conditions != null)
            {
                int[] oldTypes = new int[conditions.arraySize];
                int oldArraySize = conditions.arraySize;

                // populate old types and also check for null elements
                for (int i = 0; i < oldArraySize; i++)
                {
                    var condition = conditions.GetArrayElementAtIndex(i);
                    condition.managedReferenceValue ??= new Condition();
                    oldTypes[i] = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("m_typeIndex").intValue;
                }

                EGL.PropertyField(conditions, new GUIContent("Conditions"));
                if (oldArraySize == conditions.arraySize)
                {
                    for (int i = 0; i < conditions.arraySize; i++)
                    {
                        SerializedProperty condition = conditions.GetArrayElementAtIndex(i);
                        //
                        int oldType = condition.FindPropertyRelative("m_typeIndex").intValue;

                        // draw property drawer
                        EGL.PropertyField(condition, new GUIContent(stringTypes[oldType]));
                        //
                        // if changing type
                        int newType = condition.FindPropertyRelative("m_typeIndex").intValue;
                        if (newType != oldTypes[i])
                        {
                            Condition newCondition = System.Activator.CreateInstance(Condition.ConditionTypes[newType]) as Condition;
                            condition.managedReferenceValue = newCondition;
                            condition.FindPropertyRelative("m_typeIndex").intValue = newType;
                            m_hasChanged = true;
                        }
                        //DrawCondition(conditions.GetArrayElementAtIndex(i));
                    }
                }
            }
            else
            {
                conditions.managedReferenceValue = new Condition[0];
                EG.BeginDisabledGroup(true);
                EGL.TextField("No Conditions!");
                EG.EndDisabledGroup();
            }

            EG.indentLevel--;

            //
            // add and remove buttons
            EGL.BeginHorizontal();
            Rect addButtonRect = EG.IndentedRect(EG.IndentedRect(EGL.GetControlRect()));
            if (GUI.Button(addButtonRect, "Add Condition"))
            {
                conditions.InsertArrayElementAtIndex(conditions.arraySize);
                conditions.GetArrayElementAtIndex(conditions.arraySize - 1).managedReferenceValue = new Condition();
                m_hasChanged = true;
            }
            Rect removeButtonRect = EGL.GetControlRect();
            removeButtonRect.width -= EG.indentLevel * 5f;
            if (conditions != null && conditions.arraySize > 0 && GUI.Button(removeButtonRect, "Remove Condition"))
            {
                conditions.DeleteArrayElementAtIndex(conditions.arraySize - 1);
                m_hasChanged = true;
            }
            EGL.EndHorizontal();

            EG.indentLevel--;
            //
        }

        [System.Obsolete]
        private void DrawCondition(SerializedProperty condition)
        {
            int currentType = condition.FindPropertyRelative("m_typeIndex").intValue;
            // Condition dropdown to select which Type it is
            EGL.BeginHorizontal();
            EGL.LabelField(conditionNames[currentType], EditorStyles.boldLabel);
            int value = EGL.Popup(currentType, conditionNames);
            EGL.EndHorizontal();

            // Invert value prop
            SerializedProperty invertValue = condition.FindPropertyRelative("m_invertValue");
            EGL.PropertyField(invertValue);

            // for unique Conditions
            // Distance Condition
            SerializedProperty distance = condition.FindPropertyRelative("m_distance");
            if (distance != null)
            {
                EGL.PropertyField(distance, new GUIContent("Distance"));
            }

            // attempt the change of Type after so that there are no deletions before stuff is drawn
            if (value != currentType)
            {
                Condition newCondition = System.Activator.CreateInstance(Condition.ConditionTypes[value]) as Condition;
                condition.managedReferenceValue = newCondition;
                condition.FindPropertyRelative("m_typeIndex").intValue = value;
                m_hasChanged = true;
            }
        }
        */
    }
}
