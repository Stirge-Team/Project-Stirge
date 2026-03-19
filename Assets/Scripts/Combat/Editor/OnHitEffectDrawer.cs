using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stirge.Combat
{
    [CustomPropertyDrawer(typeof(OnHitEffect))]
    public class OnHitEffectDrawer : PropertyDrawer
    {
        private int selectedStatus;
        private readonly System.Type[] StatusTypes =
        {
            typeof(AirJuggle),
            typeof(Knockback),
            typeof(Stun)
        };

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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int totalLines = 0;

            Rect GetNewRect(Rect position)
            {
                totalLines++;
                return new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * (totalLines - 1), position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
            }

            SerializedProperty damageProp = property.FindPropertyRelative("m_damage");
            SerializedProperty statusesProp = property.FindPropertyRelative("m_statuses");

            EditorGUI.BeginProperty(position, label, property);
            Rect foldoutRect = GetNewRect(position);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
            if (property.isExpanded)
            {
                // draw damage property
                Rect damageRect = GetNewRect(position);
                EditorGUI.PropertyField(damageRect, damageProp);

                // draw Statuses array
                Rect statusesRect = GetNewRect(position);
                if (statusesProp.isExpanded)
                    totalLines += (int)(EditorGUI.GetPropertyHeight(statusesProp) / EditorGUIUtility.singleLineHeight);

                // draw popup for picking new Statuses
                Rect statusPopupRect = GetNewRect(position);
                selectedStatus = EditorGUI.Popup(statusPopupRect, selectedStatus, StringTypes);

                EditorGUI.PropertyField(statusesRect, statusesProp);

                // create new Status button
                Rect newStatusButtonRect = GetNewRect(position);
                if (GUI.Button(newStatusButtonRect, "Add new " + StringTypes[selectedStatus] + " Status"))
                {
                    Status newStatus = System.Activator.CreateInstance(StatusTypes[selectedStatus]) as Status;
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
            m_stringTypes = StatusTypes.Select(t => t.Name).ToArray();
            for (int i = 0; i < m_stringTypes.Length; i++)
            {
                // turns camel case into separate words (looks nice)
                m_stringTypes[i] = Regex.Replace(Regex.Replace(m_stringTypes[i], @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
        }
    }
}
