using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.Combat.Attacks.Serialization
{
    [InitializeOnLoad]
    public static class SerializedAttackNodeTypesCollection
    {
        private static readonly Type[] s_serializedAttackNodeTypes;
        private static readonly Type[] s_attackNodeTypes;

        static SerializedAttackNodeTypesCollection()
        {
            s_serializedAttackNodeTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                           where !domainAssembly.IsDynamic
                                           from assemblyType in domainAssembly.GetExportedTypes()
                                           where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                               && assemblyType.IsSubclassOf(typeof(SerializedAttackNode_Base))
                                           select assemblyType)
                                       .ToArray();

            int count = s_serializedAttackNodeTypes.Length;
            s_attackNodeTypes = new Type[count];

            for (int i = 0; i < count; i++)
            {
                var tempSerializedTable = (SerializedAttackNode_Base)ScriptableObject.CreateInstance(s_serializedAttackNodeTypes[i]);
                s_attackNodeTypes[i] = tempSerializedTable.attackNodeType;
                Object.DestroyImmediate(tempSerializedTable);
            }
        }

        public static IReadOnlyList<Type> attackNodeTypes => s_attackNodeTypes;

        public static Type GetSerialisedAttackNodeType(Type attackNodeType)
        {
            int index = Array.IndexOf(s_attackNodeTypes, attackNodeType);
            return index >= 0 ? s_serializedAttackNodeTypes[index] : null;
        }
    }
}
