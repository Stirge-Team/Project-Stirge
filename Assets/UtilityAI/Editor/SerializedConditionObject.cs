using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public struct SerializedConditionObject
    {
        public object value;
        public Type valueType;
        public bool isConstantValue;

        public bool setup;
    }
}
