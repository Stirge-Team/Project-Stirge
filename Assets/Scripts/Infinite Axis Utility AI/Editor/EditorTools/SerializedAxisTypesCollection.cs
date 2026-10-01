using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.EditorTools
{
    using Serialization;

    [InitializeOnLoad]
    public static class SerializedAxisTypesCollection
    {
        private static readonly Type[] s_serializedAxisTypes;
        private static readonly Type[] s_axisTypes;

        static SerializedAxisTypesCollection()
        {
            s_serializedAxisTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !domainAssembly.IsDynamic
                    from assemblyType in domainAssembly.GetExportedTypes()
                    where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                       && assemblyType.IsSubclassOf(typeof(SerializedAxis_Base))
                    select assemblyType)
                .ToArray();

            int count = s_serializedAxisTypes.Length;
            s_axisTypes = new Type[count];

            for (int i = 0; i < count; i++)
            {
                var tempSerializedTable = (SerializedAxis_Base)ScriptableObject.CreateInstance(s_serializedAxisTypes[i]);
                s_axisTypes[i] = tempSerializedTable.axisType;
                Object.DestroyImmediate(tempSerializedTable);
            }
        }

        public static IReadOnlyList<Type> axisTypes => s_axisTypes;

        public static Type GetSerializedAxisType(Type axisType)
        {
            int index = Array.IndexOf(s_axisTypes, axisType);
            return index >= 0 ? s_serializedAxisTypes[index] : null;
        }
    }
}
