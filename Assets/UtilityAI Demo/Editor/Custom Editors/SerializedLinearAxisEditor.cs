using UnityEditor;
using UnityEngine;

namespace Stirge.UtilityAI.Demo.Axes
{
    [CustomEditor(typeof(SerializedLinearAxis))]
    public class SerializedLinearAxisEditor : Editor
    {
        private const int s_samples = 20;

        private Vector2 m_domain = new(0, 1);
        private Vector2 m_range = new(0, 1);
        private AnimationCurve m_curve;

        private float m_m;
        private float m_h;
        private float m_v;

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

            float m = serializedObject.FindProperty("m_arg0").floatValue;
            if (m != m_m)
            {
                m_m = m;
                changesMade = true;
            }
            float h = serializedObject.FindProperty("m_arg1").floatValue;
            if (h != m_h)
            {
                m_h = h;
                changesMade = true;
            }
            float v = serializedObject.FindProperty("m_arg2").floatValue;
            if (v != m_v)
            {
                m_v = v;
                changesMade = true;
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

        private void UpdateCurve()
        {
            float range = m_domain.y - m_domain.x;
            float spacing = range / s_samples;

            m_curve.ClearKeys();

            bool addedLowIntersect = false;
            bool addedHighIntersect = false;
            for (int i = 0; i < s_samples + 1; i++)
            {
                float x = m_domain.x + i * spacing;
                float y = m_m * (x - m_h) + m_v;

                if (!addedLowIntersect && y < m_range.x)
                {
                    addedLowIntersect = true;
                    m_curve.AddKey((m_range.x - m_v) / m_m + m_h, m_range.x);

                }
                if (!addedHighIntersect && y > m_range.y)
                {
                    addedHighIntersect = true;
                    m_curve.AddKey((m_range.y - m_v) / m_m + m_h, m_range.y);
                }

                y = Mathf.Clamp(y, m_range.x, m_range.y);

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
