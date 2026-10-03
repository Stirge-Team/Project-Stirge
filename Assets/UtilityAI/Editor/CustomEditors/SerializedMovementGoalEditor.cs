using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
{
    using Serialization;
    using EditorTools;

    [CustomEditor(typeof(SerializedMovementGoal_Base), true)]
    public class SerializedMovementGoalEditor : SerializedScorableEditor<MovementGoal>
    {
        private const string s_scoreScalingPropertyName = "m_scoreScaling";
        private const string s_durationPropertyName = "m_duration";
        private const string s_displayNamePropertyName = "m_displayName";
        private const string s_conditionsPropertyName = "m_conditions";
        private const string s_scoringMethodsPropertyName = "m_scoringMethods";

        private static readonly string[] s_basePropertyNames = new string[]
        {
            s_scoreScalingPropertyName,
            s_durationPropertyName,
            s_displayNamePropertyName,
            s_conditionsPropertyName,
            s_scoringMethodsPropertyName
        };

        private SerializedProperty m_scoreScalingProperty;
        private SerializedProperty m_durationProperty;
        private SerializedProperty m_displayNameProperty;
        private SerializedProperty m_conditionsProperty;
        private SerializedProperty m_scoringMethodsProperty;

        protected override string[] basePropertyNames => s_basePropertyNames;
        protected override SerializedProperty conditionsProperty => m_conditionsProperty;
        protected override SerializedProperty scoringMethodsProperty => m_scoringMethodsProperty;

        protected override void FindSerializedProperties()
        {
            m_scoreScalingProperty = serializedObject.FindProperty(s_scoreScalingPropertyName);
            m_durationProperty = serializedObject.FindProperty(s_durationPropertyName);
            m_displayNameProperty = serializedObject.FindProperty(s_displayNamePropertyName);
            m_conditionsProperty = serializedObject.FindProperty(s_conditionsPropertyName);
            m_scoringMethodsProperty = serializedObject.FindProperty(s_scoringMethodsPropertyName);
        }

        protected override void DrawBaseProperties()
        {
            EGL.PropertyField(m_scoreScalingProperty);
            EGL.PropertyField(m_durationProperty);
            EGL.PropertyField(m_displayNameProperty);
        }
    }
}
