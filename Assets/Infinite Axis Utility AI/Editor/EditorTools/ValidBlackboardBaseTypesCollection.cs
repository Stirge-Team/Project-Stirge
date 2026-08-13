using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Stirge.InfiniteAxis.EditorTools
{
    [InitializeOnLoad]
    public static class ValidBlackboardBaseTypesCollection
    {
        private static readonly Type[] s_validTypes;

        static ValidBlackboardBaseTypesCollection()
        {
            s_validTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            where !domainAssembly.IsDynamic
                            from assemblyType in domainAssembly.GetExportedTypes()
                            where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                               && assemblyType.IsSubclassOf(typeof(MonoBehaviour))
                            select assemblyType)
                .ToArray();
        }

        public static IReadOnlyList<Type> ValidGenericBlackboardTypes => s_validTypes;
    }
}
