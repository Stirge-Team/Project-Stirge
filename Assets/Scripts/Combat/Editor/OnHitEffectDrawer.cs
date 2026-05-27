using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stirge.Combat
{
    using Tools;
    
    [CustomPropertyDrawer(typeof(OnHitEffect))]
    public class OnHitEffectDrawer : EasyPropertyDrawer
    {
        #region Names
        public string[] StringTypes
        {
            get
            {
                if (m_stringTypes == null || m_stringTypes.Length == 0)
                {
                    PopulateStringTypes();
                }
                return m_stringTypes;
            }
        }
        private string[] m_stringTypes;
        #endregion

        private int m_selectedStatus;

        protected override void DrawGUI(GUIContent label)
        {
            EditorGUI.EndFoldoutHeaderGroup();

            EditorGUI.BeginProperty(m_position, label, m_property);

            // Create Label
            DrawLabelHeader(label);

            if (m_property.isExpanded)
            {
                if (!PropertyIsArrayElement())
                    EditorGUI.indentLevel++;

                // draw damage property
                DrawPropertyField("m_damage");

                // draw Statuses array
                DrawPropertyField("m_statuses");

                // draw popup for picking new Statuses
                m_selectedStatus = EditorGUI.Popup(GetNewRect(), m_selectedStatus, StringTypes);

                // create new Status button
                if (GUI.Button(GetNewRect(), "Add new " + StringTypes[m_selectedStatus] + " Status"))
                {
                    SerializedProperty statusesProp = FindPropertyRelative("m_statuses");
                    Status newStatus = System.Activator.CreateInstance(Status.StatusTypes[m_selectedStatus]) as Status;
                    statusesProp.arraySize++;
                    SerializedProperty newStatusProp = statusesProp.GetArrayElementAtIndex(statusesProp.arraySize - 1);
                    newStatusProp.managedReferenceValue = newStatus;
                    newStatusProp.isExpanded = true;
                }

                if (!PropertyIsArrayElement())
                    EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1; // for foldout/label

            if (m_property.isExpanded)
            {
                totalLines += 4; // for damage, popup, and button
                totalLines += GetPropertyLineHeight("m_statuses"); // for Statuses array
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }

        private void PopulateStringTypes()
        {
            // get all the Statuses from the list and change them from CamelCase to English
            m_stringTypes = Status.StatusTypes.Select(t => t.Name).ToArray();
            for (int i = 0; i < m_stringTypes.Length; i++)
            {
                // turns camel case into separate words (looks nice)
                m_stringTypes[i] = Regex.Replace(Regex.Replace(m_stringTypes[i], @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
        }
    }
}
