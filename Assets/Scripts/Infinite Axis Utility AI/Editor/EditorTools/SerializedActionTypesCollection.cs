using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Stirge.UtilityAI.EditorTools
{
    using Serialization;
    using Action = Core.Action;

    [InitializeOnLoad]
    public static class SerializedActionTypesCollection
    {
         private static readonly Type[] s_serializedActionTypes;
         private static readonly Type[] s_actionTypes;

        static SerializedActionTypesCollection()
        {
            s_serializedActionTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !domainAssembly.IsDynamic
                    from assemblyType in domainAssembly.GetExportedTypes()
                    where !assemblyType.IsAbstract && !assemblyType.IsGenericType
                        && assemblyType.IsSubclassOf(typeof(SerializedAction_Base))
                    select assemblyType)
                .ToArray();

            int count = s_serializedActionTypes.Length;
            s_actionTypes = new Type[count];

            for (int i = 0; i < count; ++i)
            {
                var tempSerializedTable =
                    (SerializedAction_Base)ScriptableObject.CreateInstance(s_serializedActionTypes[i]);
                s_actionTypes[i] = tempSerializedTable.actionType;
                Object.DestroyImmediate(tempSerializedTable);
            }
        }

        /// <summary>
        /// <see cref="Action"/> types.
        /// </summary>
        public static IReadOnlyList<Type> actionTypes => s_actionTypes;

        /// <summary>
        /// Finds a paired serialized <see cref="Action"/> type by <see cref="Action"/> type.
        /// </summary>
        /// <param name="actionType"><see cref="Action"/> type.</param>
        /// <returns>Serialized <see cref="Action"/> type or null if not found.</returns>
        public static Type GetSerializedActionType(Type actionType)
        {
            int index = Array.IndexOf(s_actionTypes, actionType);
            return index >= 0 ? s_serializedActionTypes[index] : null;
        }
    }
}
