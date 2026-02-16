// https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-HowTo-CreateCustomInspector.html

using System;
using UnityEditor;
using UnityEngine;
using EGL = UnityEditor.EditorGUILayout;
using GL = UnityEngine.GUILayout;
using EG = UnityEditor.EditorGUI;

namespace Stirge.AI
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        private static Type[] conditionTypes =
        {
            typeof(AirJuggleCondition),
            typeof(DistanceCondition),
            typeof(GroundedCondition),
            typeof(InverterCondition),
            typeof(OffGroundCondition),
            typeof(StunnedCondition),
            typeof(TargetInRangeCondition)
        };

        private SerializedProperty m_behaviours, m_transitions;

        private bool m_hasChanged = false;

        private void OnEnable()
        {
            GetSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EG.BeginChangeCheck();

            OnGUI();

            if (EG.EndChangeCheck() || m_hasChanged)
            {
                m_hasChanged = false;
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void GetSerializedProperties()
        {
            m_behaviours = serializedObject.FindProperty(nameof(m_behaviours));
            m_transitions = serializedObject.FindProperty(nameof(m_transitions));
        }

        private void OnGUI()
        {
            // behaviours
            EGL.PropertyField(m_behaviours);

            // transitions
            EGL.LabelField(new GUIContent("Transitions"));

            // draw individual transitions
            for (int i = 0; i < m_transitions.arraySize; i++)
            {
                DrawTransition(m_transitions.GetArrayElementAtIndex(i), i);
            }
                
            // add and subtract buttons
            EGL.BeginHorizontal();
            if (GL.Button("+"))
                AddTransition();
            if (m_transitions.arraySize > 0 && GL.Button("-"))
                RemoveTransition();
            EGL.EndHorizontal();
        }

        private void AddTransition()
        {
            m_transitions.arraySize++;
        }
        private void RemoveTransition()
        {
            m_transitions.arraySize--;
        }

        private void DrawTransition(SerializedProperty transition, int id)
        {
            EGL.LabelField("Transition " + id);
            EG.indentLevel++;

            SerializedProperty targetState = transition.FindPropertyRelative("m_targetState");
            EGL.PropertyField(targetState, new GUIContent("Target State"));

            EG.indentLevel--;
        }

        private void DrawCondition(SerializedProperty condition)
        {
            var inverterCondition = condition.FindPropertyRelative("m_condition");
            if (inverterCondition != null)
            {
                EGL.LabelField("Inverter Condition");
                DrawCondition(inverterCondition);
                return;
            }


        }
    }
}
