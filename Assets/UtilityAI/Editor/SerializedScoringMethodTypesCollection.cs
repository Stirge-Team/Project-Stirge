using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.EditorTools
{
    [InitializeOnLoad]
    public class SerializedScoringMethodTypesCollection
    {
        private static readonly Type[] s_serializedScoringMethodTypes;
        private static readonly Type[] s_scoringMethodTypes;

        static SerializedScoringMethodTypesCollection()
        {
            s_serializedScoringMethodTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     where !domainAssembly.IsDynamic
                                     from assemblyType in domainAssembly.GetExportedTypes()
                                     where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                        && assemblyType.IsSubclassOf(typeof(SerializedScoringMethod_Base))
                                     select assemblyType)
                .ToArray();

            int count = s_serializedScoringMethodTypes.Length;
            s_scoringMethodTypes = new Type[count];

            for (int i = 0; i < count; i++)
            {
                var tempSerializedTable = (SerializedScoringMethod_Base)ScriptableObject.CreateInstance(s_serializedScoringMethodTypes[i]);
                s_scoringMethodTypes[i] = tempSerializedTable.scoringMethodType;
                Object.DestroyImmediate(tempSerializedTable);
            }
        }

        public static IReadOnlyList<Type> scoringMethodTypes => s_scoringMethodTypes;

        public static Type GetSerializedScoringMethodType(Type scoringMethodType)
        {
            int index = Array.IndexOf(s_scoringMethodTypes, scoringMethodType);
            return index >= 0 ? s_serializedScoringMethodTypes[index] : null;
        }
    }
}
