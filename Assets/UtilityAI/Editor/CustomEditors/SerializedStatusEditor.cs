using System;
using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;

namespace Stirge.UtilityAI.CustomEditors
{
    [CustomEditor(typeof(SerializedStatus_Base), true)]
    public class SerializedStatusEditor : SerializedScorableEditor<Status>
    {
        private const string s_scoreScalingPropertyName = "m_scoreScaling";
        private const string s_displayNamePropertyName = "m_displayName";
        private const string s_stackTypePropertyName = "m_stackType";
        private const string s_maxStacksPropertyName = "m_maxStacks";
        private const string s_durationTypePropertyName = "m_durationType";
        private const string s_conditionsPropertyName = "m_conditions";
        private const string s_scoringMethodsPropertyName = "m_scoringMethods";

        private static readonly string[] s_basePropertyNames = new string[]
        {
            s_scoreScalingPropertyName,
            s_displayNamePropertyName,
            s_stackTypePropertyName,
            s_maxStacksPropertyName,
            s_durationTypePropertyName,
            s_conditionsPropertyName,
            s_scoringMethodsPropertyName
        };

        private SerializedProperty m_scoreScalingProperty;
        private SerializedProperty m_displayNameProperty;
        private SerializedProperty m_stackTypeProperty;
        private SerializedProperty m_maxStacksProperty;
        private SerializedProperty m_durationTypeProperty;
        private SerializedProperty m_conditionsProperty;
        private SerializedProperty m_scoringMethodsProperty;

        protected override string[] basePropertyNames => s_basePropertyNames;
        protected override SerializedProperty conditionsProperty => m_conditionsProperty;
        protected override SerializedProperty scoringMethodsProperty => m_scoringMethodsProperty;

        private DrawData m_drawStackData = DrawData.Waiting;
        private DrawData m_drawDurationData = DrawData.Waiting;

        protected override void FindSerializedProperties()
        {
            m_scoreScalingProperty = serializedObject.FindProperty(s_scoreScalingPropertyName);
            m_displayNameProperty = serializedObject.FindProperty(s_displayNamePropertyName);
            m_stackTypeProperty = serializedObject.FindProperty(s_stackTypePropertyName);
            m_maxStacksProperty = serializedObject.FindProperty(s_maxStacksPropertyName);
            m_durationTypeProperty = serializedObject.FindProperty(s_durationTypePropertyName);
            m_conditionsProperty = serializedObject.FindProperty(s_conditionsPropertyName);
            m_scoringMethodsProperty = serializedObject.FindProperty(s_scoringMethodsPropertyName);
        }

        protected override void DrawBaseProperties()
        {
            EGL.PropertyField(m_scoreScalingProperty);
            EGL.PropertyField(m_displayNameProperty);

            // stack data
            if (m_drawStackData == DrawData.Waiting)
            {
                if (DeclaresPropertyAtAll(nameof(Status.StackType)))
                {
                    m_drawStackData = DrawData.DoNot;
                }
                else
                {
                    m_drawStackData = DrawData.Do;
                }
            }
            if (m_drawStackData == DrawData.Do)
            {
                EGL.PropertyField(m_stackTypeProperty);
                // if Stackable
                if ((StatusStackType)m_stackTypeProperty.intValue == StatusStackType.Stackable)
                    EGL.PropertyField(m_maxStacksProperty);
            }

            // duration data
            if (m_drawDurationData == DrawData.Waiting)
            {
                if (DeclaresPropertyAtAll(nameof(Status.DurationType)))
                {
                    m_drawDurationData = DrawData.DoNot;
                }
                else
                {
                    m_drawDurationData = DrawData.Do;
                }
            }
            if (m_drawDurationData == DrawData.Do)
            {
                EGL.PropertyField(m_durationTypeProperty);
            }
        }

        private bool DeclaresPropertyAtAll(string propertyName)
        {
            return ((SerializedStatus_Base)target).StatusType.GetProperty(propertyName).DeclaringType != typeof(Status);
        }

        private enum DrawData
        {
            Waiting,
            DoNot,
            Do
        }
    }
}
