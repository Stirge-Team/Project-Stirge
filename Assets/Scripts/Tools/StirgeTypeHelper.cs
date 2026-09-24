using System;
using System.Collections.Generic;

namespace Stirge.Tools
{
    public static class StirgeTypeHelper
    {
        static StirgeTypeHelper()
        {
            
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

        private static readonly Dictionary<string, Type> s_builtInTypeNameToType = new()
        {
            { "bool", typeof(Boolean) },
            { "byte", typeof(Byte) },
            { "sbyte", typeof(SByte) },
            { "char", typeof(Char) },
            { "decimal", typeof(Decimal) },
            { "double", typeof(Double) },
            { "float", typeof(Single) },
            { "int", typeof(Int32) },
            { "uint", typeof(UInt32) },
            { "nint", typeof(IntPtr) },
            { "nuint", typeof(UIntPtr) },
            { "long", typeof(Int64) },
            { "ulong", typeof(UInt64) },
            { "short", typeof(Int16) },
            { "ushort", typeof(UInt16) },
            { "object", typeof(System.Object) },
            { "string", typeof(String) },
            { "delegate", typeof(Delegate) },
            { "dynamic", typeof(System.Object) }
        };
    }
}
