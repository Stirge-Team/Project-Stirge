using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EGL = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.CustomEditors
{
    using System.Linq;
    using Tools;

    [CustomEditor(typeof(SerializedCondition))]
    public class SerializedConditionEditor : Editor
    {
        private static string s_operationPropertyName = "m_operation";
        private static string s_firstObjectPropertyName = "m_firstObject";
        private static string s_secondObjectPropertyName = "m_secondObject";

        private static Type[] s_validConstantTypes = StirgeTypeHelper.DataTypes.ToArray();

        private SerializedProperty m_operationProperty;
        private SerializedProperty m_firstObjectProperty;
        private SerializedProperty m_secondObjectProperty;

        private SerializedConditionObject m_firstObject;
        private SerializedConditionObject m_secondObject;

        private string m_stringTypeName;

        private void OnEnable()
        {
            m_operationProperty = serializedObject.FindProperty(s_operationPropertyName);
            m_firstObjectProperty = serializedObject.FindProperty(s_firstObjectPropertyName);
            m_secondObjectProperty = serializedObject.FindProperty(s_secondObjectPropertyName);

            // init structs
            if (!m_firstObject.setup)
            {
                m_firstObject.setup = true;
                m_firstObject.value = m_firstObjectProperty.managedReferenceValue;
                if (m_firstObject.value != null)
                {
                    m_firstObject.isConstantValue = false;
                    m_firstObject.valueType = m_firstObject.value.GetType();
                }
            }
            if (!m_secondObject.setup)
            {
                m_secondObject.setup = true;
                m_secondObject.value = m_secondObjectProperty.managedReferenceValue;
                if (m_secondObject.value != null)
                {
                    m_secondObject.isConstantValue = false;
                    m_secondObject.valueType = m_secondObject.value.GetType();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            EGL.PropertyField(m_operationProperty);

            EGL.LabelField(new GUIContent("First Object"), EditorStyles.boldLabel);
            DrawObject(m_firstObjectProperty, ref m_firstObject);
            EGL.LabelField(new GUIContent("Second Object"), EditorStyles.boldLabel);
            DrawObject(m_secondObjectProperty, ref m_secondObject);
        }

        private void DrawObject(SerializedProperty property, ref SerializedConditionObject obj)
        {
            bool isConstantToggle = EGL.Toggle(new GUIContent("Constant Value"), obj.isConstantValue);
            if (isConstantToggle != obj.isConstantValue)
            {
                obj.isConstantValue = isConstantToggle;
            }

            // editor for constant values
            if (obj.isConstantValue)
            {
                // type options

                // if selected type is not null AND is valid constant type
                if (obj.valueType != null && StirgeTypeHelper.IsDataType(obj.valueType))
                {

                }
                else
                {
                    EGL.HelpBox("Please select a valid type for the Constant Value.", MessageType.Warning);
                }
            }
            // editor for non-constant values
            else
            {
                string typeName = EGL.TextField(m_stringTypeName);
                Type type = Type.GetType(typeName);
                if (type != null)
                {
                    Object newObject = EGL.ObjectField(obj.value as Object, type);
                    obj.valueType = type;

                }
                else
                {
                    EGL.HelpBox("Please select a valid type for the Reference Value.", MessageType.Warning);
                }
            }
        }
    }
}
