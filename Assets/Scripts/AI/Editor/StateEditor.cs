// https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-HowTo-CreateCustomInspector.html

using UnityEditor;
using UnityEngine;
using EGL = UnityEditor.EditorGUILayout;
using GL = UnityEngine.GUILayout;
using EG = UnityEditor.EditorGUI;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stirge.AI
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        // static list of all valid Conditions
        private static readonly System.Type[] conditionTypes =
        {
            typeof(Condition),
            typeof(AirJuggleCondition),
            typeof(DistanceCondition),
            typeof(GroundedCondition),
            typeof(OffGroundCondition),
            typeof(StunnedCondition),
            typeof(TargetInRangeCondition)
        };
        // to display list of Condition names
        private static string[] stringTypes;

        private bool m_hasChanged = false;

        private void OnEnable()
        {
            // get all the Conditions from the static list and change them from CamelCase to English
            stringTypes = conditionTypes.Select(t => t.Name).ToArray();
            for (int i = 0; i < stringTypes.Length; i++)
            {
                // turns camel case into separate words (looks nice)
                stringTypes[i] = Regex.Replace(Regex.Replace(stringTypes[i], @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
            // show Condition as this
            stringTypes[0] = "Empty Condition";
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
            SerializedProperty behaviours = serializedObject.FindProperty("m_behaviours");
            EGL.PropertyField(behaviours);

            // draw all Transitions
            SerializedProperty transitions = serializedObject.FindProperty("m_transitions");
            EGL.LabelField(new GUIContent("Transitions"), EditorStyles.boldLabel);
            if (transitions != null && transitions.arraySize > 0)
            {
                for (int i = 0; i < transitions.arraySize; i++)
                {
                    DrawTransition(transitions.GetArrayElementAtIndex(i), i);
                }
            }
            else
            {
                EG.BeginDisabledGroup(true);
                EGL.TextField("No Transitions!");
                EG.EndDisabledGroup();
            }
                
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
        }

        private void DrawTransition(SerializedProperty transition, int index)
        {           
            EGL.LabelField("Transition " + index);
            EG.indentLevel++;

            // Target State field
            SerializedProperty targetState = transition.FindPropertyRelative("m_targetState");
            EGL.PropertyField(targetState, new GUIContent("Target State"));

            // draw all Conditions
            SerializedProperty conditions = transition.FindPropertyRelative("m_conditions");
            EGL.LabelField(new GUIContent("Conditions"));
            EG.indentLevel++;
            if (conditions != null)
            {
                for (int i = 0; i < conditions.arraySize; i++)
                {
                    DrawCondition(conditions.GetArrayElementAtIndex(i));
                }
            }
            else
            {
                EG.BeginDisabledGroup(true);
                EGL.TextField("No Conditions!");
                EG.EndDisabledGroup();
            }

            EG.indentLevel--;

            // add and remove buttons
            EGL.BeginHorizontal();
            if (GL.Button("Add Condition"))
            {
                conditions.InsertArrayElementAtIndex(conditions.arraySize);
                conditions.GetArrayElementAtIndex(conditions.arraySize - 1).managedReferenceValue = new Condition();
                m_hasChanged = true;
            }
            if (conditions != null && conditions.arraySize > 0 && GL.Button("Remove Condition"))
            {
                conditions.DeleteArrayElementAtIndex(conditions.arraySize - 1);
                m_hasChanged = true;
            }
            EGL.EndHorizontal();

            EG.indentLevel--;
        }

        private void DrawCondition(SerializedProperty condition, string label = "Condition")
        {
            // Condition dropdown to select which Type it is
            EGL.BeginHorizontal();
            EGL.LabelField(label);
            SerializedProperty typeIndex = condition.FindPropertyRelative("m_typeIndex");
            int value = EGL.Popup(typeIndex.intValue, stringTypes);
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
            if (value != typeIndex.intValue)
            {
                Condition newCondition = System.Activator.CreateInstance(conditionTypes[value]) as Condition;
                condition.managedReferenceValue = newCondition;
                condition.FindPropertyRelative("m_typeIndex").intValue = value;
                m_hasChanged = true;
            }
        }
    }
}
