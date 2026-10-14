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
        private const string s_durationPropertyName = "m_duration";
        private const string s_inflictConditionPropertyName = "m_inflictCondition";
        private const string s_inflictDelayPropertyName = "m_inflictDelay";
        private const string s_conditionsPropertyName = "m_conditions";
        private const string s_scoringMethodsPropertyName = "m_scoringMethods";

        private static readonly string[] s_basePropertyNames = new string[]
        {
            s_scoreScalingPropertyName,
            s_displayNamePropertyName,
            s_stackTypePropertyName,
            s_maxStacksPropertyName,
            s_durationTypePropertyName,
            s_durationPropertyName,
            s_inflictConditionPropertyName,
            s_inflictDelayPropertyName,
            s_conditionsPropertyName,
            s_scoringMethodsPropertyName
        };

        private SerializedProperty m_scoreScalingProperty;
        private SerializedProperty m_displayNameProperty;
        private SerializedProperty m_stackTypeProperty;
        private SerializedProperty m_maxStacksProperty;
        private SerializedProperty m_durationTypeProperty;
        private SerializedProperty m_durationProperty;
        private SerializedProperty m_inflictConditionProperty;
        private SerializedProperty m_inflictDelayProperty;
        private SerializedProperty m_conditionsProperty;
        private SerializedProperty m_scoringMethodsProperty;

        protected override string[] basePropertyNames => s_basePropertyNames;
        protected override SerializedProperty conditionsProperty => m_conditionsProperty;
        protected override SerializedProperty scoringMethodsProperty => m_scoringMethodsProperty;

        protected override void FindSerializedProperties()
        {
            m_scoreScalingProperty = serializedObject.FindProperty(s_scoreScalingPropertyName);
            m_displayNameProperty = serializedObject.FindProperty(s_displayNamePropertyName);
            m_stackTypeProperty = serializedObject.FindProperty(s_stackTypePropertyName);
            m_maxStacksProperty = serializedObject.FindProperty(s_maxStacksPropertyName);
            m_durationTypeProperty = serializedObject.FindProperty(s_durationTypePropertyName);
            m_durationProperty = serializedObject.FindProperty(s_durationPropertyName);
            m_inflictConditionProperty = serializedObject.FindProperty(s_inflictConditionPropertyName);
            m_inflictDelayProperty = serializedObject.FindProperty(s_inflictDelayPropertyName);
            m_conditionsProperty = serializedObject.FindProperty(s_conditionsPropertyName);
            m_scoringMethodsProperty = serializedObject.FindProperty(s_scoringMethodsPropertyName);
        }

        protected override void DrawBaseProperties()
        {
            EGL.PropertyField(m_scoreScalingProperty);
            EGL.PropertyField(m_displayNameProperty);
            EGL.PropertyField(m_stackTypeProperty);
            EGL.PropertyField(m_maxStacksProperty);
            EGL.PropertyField(m_durationTypeProperty);
            EGL.PropertyField(m_durationProperty);
            EGL.PropertyField(m_inflictConditionProperty);
            EGL.PropertyField(m_inflictDelayProperty);
        }
    }
}
