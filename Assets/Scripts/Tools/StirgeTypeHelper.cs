using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Tools
{
    public static class StirgeTypeHelper
    {
        static StirgeTypeHelper()
        {
            DataTypes.UnionWith(NumericTypes);
        }

        public static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(Byte), typeof(SByte), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(Int16), typeof(Int32), typeof(Int64),
            typeof(Decimal), typeof(Double), typeof(Single)
        };

        public static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type);
        }

        public static readonly HashSet<Type> DataTypes = new HashSet<Type>
        {
            typeof(String), typeof(Vector2), typeof(Vector3)
        };

        public static bool IsDataType(Type type)
        {
            return DataTypes.Contains(type);
        }
    }
}
