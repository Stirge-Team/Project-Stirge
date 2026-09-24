using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Stirge.GenericBlackboard.CustomEditors
{
    using EditorTools;
    using System;
    using Tools;

    [CustomPropertyDrawer(typeof(BlackboardPropertyName))]
    public class BlackboardPropertyNamePropertyDrawer : EasyPropertyDrawer
    {
        private static Type s_blackboardBaseType;

        protected override void DrawGUI(GUIContent label)
        {         
            EditorGUI.BeginProperty(m_position, label, m_property);

            DrawPropertyField("m_propertyName", label);

            using (new EditorGUI.DisabledScope(true))
            {
                string typeName = s_blackboardBaseType != null ? s_blackboardBaseType.AssemblyQualifiedName : string.Empty;
                GUIStyle style = new(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter };
                if (typeName.Contains(','))
                    EditorGUI.TextField(GetNewRect(), typeName[..typeName.IndexOf(',')], style);
                else
                    EditorGUI.TextField(GetNewRect(), typeName, style);
            }
            if (GUI.Button(GetNewRect(), new GUIContent("Blackboard Base Type")))
            {
                SelectType();
            }
            if (s_blackboardBaseType != null && GUI.Button(GetNewRect(), new GUIContent("Choose Property")))
            {
                AddProperty();
            }

            EditorGUI.EndProperty();
        }

        private void SelectType()
        {
            var genericMenu = new GenericMenu();

            for (int i = 0, count = ValidBlackboardBaseTypesCollection.ValidGenericBlackboardTypes.Count; i < count; i++)
            {
                Type type = ValidBlackboardBaseTypesCollection.ValidGenericBlackboardTypes[i];
                string typeName = type.Name;
                genericMenu.AddItem(new GUIContent(typeName), false, () =>
                {
                    s_blackboardBaseType = type;
                });
            }

            genericMenu.ShowAsContext();
        }

        private void AddProperty()
        {
            var genericMenu = new GenericMenu();

            // get type of blackboard
            Type blackboardType = typeof(GenericBlackboard<>).MakeGenericType(s_blackboardBaseType);

            // call this blackboard's static constructor to initialise values
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(blackboardType.TypeHandle);

            // get the CachedPropertyInfosArray from the GenericBlackboard
            // GetFields() returns an array of all the Fields of a class. GenericBlackboard only has one field, the CachedPropertyInfosArray, thus index 0.
            // GetValue(null) returns the value of that field. The parameter is the instance to get the value from. Because GenericBlackboard
            // is a static class, there are no instances of the class, thus we pass 'null'.
            PropertyInfo[] propertyInfos = blackboardType.GetFields()[0].GetValue(null) as PropertyInfo[]; // just trust me

            for (int i = 0, count = propertyInfos.Length; i < count; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
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
            int lines = 3;
            if (s_blackboardBaseType != null)
                lines++;
            return lines;
        }
    }
}
