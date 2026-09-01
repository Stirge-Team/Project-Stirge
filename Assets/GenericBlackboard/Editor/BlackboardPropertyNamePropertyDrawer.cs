using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Stirge.InfiniteAxis.CustomEditors
{
    using GenericBlackboard;
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
            dynamic tempBlackboard = Activator.CreateInstance(blackboardType);

            PropertyInfo[] propertyInfos = tempBlackboard.CachedPropertyInfosArray;

            for (int i = 0, count = propertyInfos.Length; i < count; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                string name = propertyInfo.Name;
                genericMenu.AddItem(new GUIContent(name + $" : {propertyInfo.PropertyType.Name}"), false, () =>
                {
                    FindPropertyRelative("m_propertyName").stringValue = name;
                    FindPropertyRelative("m_hash").intValue = BlackboardPropertyName.GetHashCode(name);
                    FindPropertyRelative("m_type").boxedValue = propertyInfo.PropertyType; // I think this works

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
