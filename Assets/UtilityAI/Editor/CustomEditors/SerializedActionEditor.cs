using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;

namespace Stirge.UtilityAI.CustomEditors
{
    [CustomEditor(typeof(SerializedAction), true)]
    public class SerializedActionEditor : SerializedScorableEditor<Action>
    {
        private const string s_scoreScalingPropertyName = "m_scoreScaling";
        private const string s_durationPropertyName = "m_duration";
        private const string s_displayNamePropertyName = "m_displayName";
        private const string s_actionTypePropertyName = "m_actionType";
        private const string s_timelineAssetPropertyName = "m_timeline";
        private const string s_damagePropertyName = "m_damage";
        private const string s_rangePropertyName = "m_range";
        private const string s_statusesPropertyName = "m_statuses";
        private const string s_conditionsPropertyName = "m_conditions";
        private const string s_scoringMethodsPropertyName = "m_scoringMethods";

        private static readonly string[] s_basePropertyNames = new string[10]
        {
            s_scoreScalingPropertyName,
            s_durationPropertyName,
            s_displayNamePropertyName,
            s_actionTypePropertyName,
            s_timelineAssetPropertyName,
            s_damagePropertyName,
            s_rangePropertyName,
            s_statusesPropertyName,
            s_conditionsPropertyName,
            s_scoringMethodsPropertyName
        };

        private SerializedProperty m_scoreScalingProperty;
        private SerializedProperty m_durationProperty;
        private SerializedProperty m_displayNameProperty;
        private SerializedProperty m_actionTypeProperty;
        private SerializedProperty m_timelineAssetProperty;
        private SerializedProperty m_damageProperty;
        private SerializedProperty m_rangeProperty;
        private SerializedProperty m_statusesProperty;
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
            m_actionTypeProperty = serializedObject.FindProperty(s_actionTypePropertyName);
            m_timelineAssetProperty = serializedObject.FindProperty(s_timelineAssetPropertyName);
            m_damageProperty = serializedObject.FindProperty(s_damagePropertyName);
            m_rangeProperty = serializedObject.FindProperty(s_rangePropertyName);
            m_statusesProperty = serializedObject.FindProperty(s_statusesPropertyName);
            m_conditionsProperty = serializedObject.FindProperty(s_conditionsPropertyName);
            m_scoringMethodsProperty = serializedObject.FindProperty(s_scoringMethodsPropertyName);
        }

        protected override void DrawBaseProperties()
        {
            EGL.PropertyField(m_scoreScalingProperty);
            EGL.PropertyField(m_durationProperty);
            EGL.PropertyField(m_displayNameProperty);
            EGL.PropertyField(m_actionTypeProperty);
            EGL.PropertyField(m_timelineAssetProperty);
            EGL.PropertyField(m_damageProperty);
            EGL.PropertyField(m_rangeProperty);
            EGL.PropertyField(m_statusesProperty);
        }
    }
}
