using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Stirge.InfiniteAxis.CustomEditors
{
    using Blackboard;
    using Stirge.InfiniteAxis.Serialization;
    using System;
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

            SerializedActor actorData = (SerializedActor)m_property.serializedObject.context;
            Type targetType = actorData.targetType;
            Type blackboardType = typeof(GenericBlackboard<>).MakeGenericType(targetType);
            var tempBlackboard = (GenericBlackboard_Base)Activator.CreateInstance(blackboardType);

            for (int i = 0, count = tempBlackboard.GetCachedPropertyInfosArray.Length; i < count; i++)
            {
                PropertyInfo propertyInfo = tempBlackboard.GetCachedPropertyInfosArray[i];
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
