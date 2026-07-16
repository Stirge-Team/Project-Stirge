using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Stirge.UtilityAI.CustomEditors
{
    using Blackboard;
    using Core;
    using Tools;

    [CustomPropertyDrawer(typeof(BlackboardPropertyName))]
    public class BlackboardPropertyNamePropertyDrawer : EasyPropertyDrawer
    {
        protected override void DrawGUI(GUIContent label)
        {         
            EditorGUI.BeginProperty(m_position, label, m_property);

            DrawPropertyField("m_propertyName", label);

            if (GUI.Button(GetNewRect(), new GUIContent("Choose Property")))
            {
                AddProperty();
            }

            EditorGUI.EndProperty();
        }

        private void AddProperty()
        {
            var genericMenu = new GenericMenu();

            for (int i = 0, count = EnemyBlackboard.CachedPropertyInfosArray.Length; i < count; i++)
            {
                PropertyInfo propertyInfo = EnemyBlackboard.CachedPropertyInfosArray[i];
                string name = propertyInfo.Name;
                genericMenu.AddItem(new GUIContent(name + $" : {propertyInfo.PropertyType.Name}"), false, () =>
                {
                    FindPropertyRelative("m_propertyName").stringValue = name;
                    FindPropertyRelative("m_hash").intValue = BlackboardPropertyName.GetHashCode(name);

                    m_property.serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                });
            }

            genericMenu.ShowAsContext();
        }

        protected override int GetHeight(GUIContent label)
        {
            return 2;
        }
    }
}
