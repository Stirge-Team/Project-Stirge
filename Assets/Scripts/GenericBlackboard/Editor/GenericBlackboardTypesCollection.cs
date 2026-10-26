using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Stirge.GenericBlackboard
{
    public static class GenericBlackboardTypesCollection<TBase> where TBase : MonoBehaviour
    {
        private static readonly Type[] s_types;

        static GenericBlackboardTypesCollection()
        {
            List<Type> types = new();
            PropertyInfo[] propertyInfos = GenericBlackboard<TBase>.CachedPropertyInfosArray;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!types.Contains(propertyInfo.PropertyType))
                {
                    types.Add(propertyInfo.PropertyType);
                }
            }
            s_types = types.ToArray();
        }

        public static IReadOnlyList<Type> UsedTypes => s_types;
    }
}
