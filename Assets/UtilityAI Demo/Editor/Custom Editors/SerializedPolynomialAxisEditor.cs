using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Stirge.UtilityAI.Demo.Axes
{
    [CustomEditor(typeof(SerializedPolynomialAxis))]
    public class SerializedPolynomialAxisEditor : Editor
    {
        private const int s_samples = 100;

        private Vector2 m_domain = new(0, 1);
        private Vector2 m_range = new(0, 1);
        private AnimationCurve m_curve;

        private int m_polynomialType;
        private float[] m_params = new float[0];

        private void OnEnable()
        {
            m_curve ??= new();
            UpdateCurve();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();

            bool changesMade = false;
            EditorGUILayout.LabelField(new GUIContent("Preview"), EditorStyles.boldLabel);

            int polynomialType = serializedObject.FindProperty("m_arg0").intValue;
            if (polynomialType != m_polynomialType)
            {
                m_polynomialType = polynomialType;
                changesMade = true;
            }
            SerializedProperty paramsProperty = serializedObject.FindProperty("m_arg1");
            int count = paramsProperty.arraySize;
            if (m_params.Length != count)
            {
                m_params = new float[count];
                changesMade = true;
            }
            for (int i = 0; i < count; i++)
            {
                float newValue = paramsProperty.GetArrayElementAtIndex(i).floatValue;
                if (m_params[i] != newValue)
                {
                    m_params[i] = newValue;
                    changesMade = true;
                }
            }

            Vector2 newDomain = EditorGUILayout.Vector2Field("Preview Domain", m_domain);
            if (newDomain.x > newDomain.y)
                newDomain.x = newDomain.y;
            if (newDomain.y < newDomain.x)
                newDomain.y = newDomain.x;
            if (newDomain != m_domain)
            {
                m_domain = newDomain;
                changesMade = true;
            }

            Vector2 newRange = EditorGUILayout.Vector2Field("Preview Range", m_range);
            if (newRange.x > newRange.y)
                newRange.x = newRange.y;
            if (newRange.y < newRange.x)
                newRange.y = newRange.x;
            if (newRange != m_range)
            {
                m_range = newRange;
                changesMade = true;
            }

            if (changesMade)
                UpdateCurve();

            int paramCount = m_polynomialType + 1;
            if (paramCount != m_params.Length)
                EditorGUILayout.LabelField("Parameter count is invalid for this Polynomial Type, cannot draw preview.", EditorStyles.boldLabel);
            else
            {
                EditorGUILayout.CurveField(m_curve, Color.green, new Rect(m_domain.x, m_range.x, m_domain.y - m_domain.x, m_range.y - m_range.x), GUILayout.Height(EditorGUIUtility.singleLineHeight * 8));

                EditorGUILayout.BeginHorizontal();
                float range = m_domain.y - m_domain.x;
                float spacing = range / 10;
                for (int i = 0; i < 11; i++)
                {
                    EditorGUILayout.LabelField((m_domain.x + spacing * i).ToString("0.##"), GUILayout.Width((Screen.width - 95) / 10 + 1));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void UpdateCurve()
        {
            float GetY(float x)
            {
                // build terms
                float value = 0;
                for (int i = 0, count = m_params.Length; i < count; i++)
                {
                    value += m_params[i] * Mathf.Pow(x, count - i - 1);
                }

                return value;
            }
            
            float range = m_domain.y - m_domain.x;
            float spacing = range / s_samples;

            m_curve.ClearKeys();

            for (int i = 0; i < s_samples + 1; i++)
            {
                float x = m_domain.x + i * spacing;
                float y = GetY(x);

                m_curve.AddKey(x, y);
            }
            for (int i = 0, count = m_curve.length; i < count; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(m_curve, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(m_curve, i, AnimationUtility.TangentMode.Linear);
            }
        }
    }
}
