using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.Callbacks
{
    [InitializeOnLoad]
    public static class SerializableCallbackTypesCollection
    {
        private static readonly Type[] s_serializableCallbackTypes;
        private static readonly Type[] s_callbackTypes;

        static SerializableCallbackTypesCollection()
        {
            s_serializableCallbackTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                               where !domainAssembly.IsDynamic
                               from assemblyType in domainAssembly.GetExportedTypes()
                               where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                                   && assemblyType.IsSubclassOf(typeof(CallbackBinding_Base))
                               select assemblyType)
                .ToArray();

            int count = s_serializableCallbackTypes.Length;
            s_callbackTypes = new Type[count];

            for (int i = 0; i < count; i++)
            {
                var tempCallback = (CallbackBinding_Base)ScriptableObject.CreateInstance(s_serializableCallbackTypes[i]);
                s_callbackTypes[i] = tempCallback.valueType;
                Object.DestroyImmediate(tempCallback);
            }
        }

        public static IReadOnlyList<Type> callbackTypes => s_callbackTypes;

        public static Type GetSerializableCallbackType(Type callbackType)
        {
            int index = Array.IndexOf(s_callbackTypes, callbackType);
            return index >= 0 ? s_serializableCallbackTypes[index] : null;
        }
    }
}