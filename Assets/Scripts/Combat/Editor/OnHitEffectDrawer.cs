using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stirge.Combat
{
    [CustomPropertyDrawer(typeof(OnHitEffect))]
    public class OnHitEffectDrawer : PropertyDrawer
    {
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

        private int m_selectedStatus;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int totalLines = 0;

            void DrawPropertyField(string propertyName)
            {
                SerializedProperty propToDraw = property.FindPropertyRelative(propertyName);
                Rect propRect = GetNewRect();
                EditorGUI.PropertyField(propRect, propToDraw);

                if (propToDraw.propertyType == SerializedPropertyType.Float)
                {
                    if (propToDraw.floatValue < 0)
                        propToDraw.floatValue = 0;
                }
            }

            Rect GetNewRect()
            {
                totalLines++;
                return new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * (totalLines - 1), position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
            }

            EditorGUI.EndFoldoutHeaderGroup();

            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(GetNewRect(), property.isExpanded, label);
            if (property.isExpanded)
            {
                // draw damage property
                DrawPropertyField("m_damage");

                // draw Statuses array
                DrawPropertyField("m_statuses");

                // add extra lines for height of array
                SerializedProperty statusesProp = property.FindPropertyRelative("m_statuses");
                if (statusesProp.isExpanded)
                    totalLines += (int)(EditorGUI.GetPropertyHeight(statusesProp) / EditorGUIUtility.singleLineHeight);

                // draw popup for picking new Statuses
                m_selectedStatus = EditorGUI.Popup(GetNewRect(), m_selectedStatus, StringTypes);

                // create new Status button
                if (GUI.Button(GetNewRect(), "Add new " + StringTypes[m_selectedStatus] + " Status"))
                {
                    Status newStatus = System.Activator.CreateInstance(Status.StatusTypes[m_selectedStatus]) as Status;
                    statusesProp.arraySize++;
                    SerializedProperty newStatusProp = statusesProp.GetArrayElementAtIndex(statusesProp.arraySize - 1);
                    newStatusProp.managedReferenceValue = newStatus;
                    newStatusProp.isExpanded = true;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (property.isExpanded)
            {
                totalLines += 4; // for damage, Status label, popup, and button

                SerializedProperty statusesProp = property.FindPropertyRelative("m_statuses");
                totalLines += (int)(EditorGUI.GetPropertyHeight(statusesProp) / EditorGUIUtility.singleLineHeight);
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
