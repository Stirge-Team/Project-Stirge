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
    using Tools;
    
    [CustomEditor(typeof(State))]
    public class StateEditor : EasyCustomEditor
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

        protected override void OnEnableThis()
        {
            PopulateBehaviourNames();
            PopulateConditionNames();
        }

        protected override void OnGUI()
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
                behavioursProp.isExpanded = true;
                newBehaviourProp.isExpanded = true;
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
    }
}
