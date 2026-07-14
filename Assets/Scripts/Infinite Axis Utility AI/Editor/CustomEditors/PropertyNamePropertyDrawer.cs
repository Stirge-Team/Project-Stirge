using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Stirge.UtilityAI.CustomEditors
{
    using Core;
    using Tools;

    [CustomPropertyDrawer(typeof(BlackboardPropertyName))]
    public class PropertyNamePropertyDrawer : EasyPropertyDrawer
    {
        protected override void DrawGUI(GUIContent label)
        {
            EditorGUI.BeginProperty(m_position, label, m_property);

            DrawPropertyField("m_propertyName");

            if (GUI.Button(GetNewRect(), new GUIContent("Set Property")))
            {
                AddProperty();
            }

            EditorGUI.EndProperty();
        }

        private void AddProperty()
        {
            var genericMenu = new GenericMenu();

            for (int i = 0, count = UtilityEnemy.CachedPropertyInfosArray.Length; i < count; i++)
            {
                PropertyInfo propertyInfo = UtilityEnemy.CachedPropertyInfosArray[i];
                string name = propertyInfo.Name;
                genericMenu.AddItem(new GUIContent(name), false, () =>
                {
                    FindPropertyRelative("m_propertyName").stringValue = name;
                    FindPropertyRelative("m_hash").intValue = name.GetHashCode();

                    m_property.serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                }
                );
            }

            genericMenu.ShowAsContext();
        }

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 2; // for string and button

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
