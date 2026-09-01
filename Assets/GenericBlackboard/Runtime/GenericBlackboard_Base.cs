using UnityEngine;

namespace Stirge.GenericBlackboard
{
    using System;
    using System.Reflection;

    public abstract class GenericBlackboard_Base
    {
        public abstract PropertyInfo[] GetCachedPropertyInfosArray { get; }
        
        public abstract bool TryGetStructValue<T>(BlackboardPropertyName propertyName, out T value) where T : struct;
        public abstract bool TryGetClassValue<T>(BlackboardPropertyName propertyName, out T value) where T : class;
        public abstract bool TryGetObjectValue(Type valueType, BlackboardPropertyName propertyName, out object value);

        public abstract void SetStructValue<T>(BlackboardPropertyName propertyName, T value) where T : struct;
        public abstract void SetClassValue<T>(BlackboardPropertyName propertyName, T value) where T : class;
        public abstract void SetObjectValue(Type valueType, BlackboardPropertyName propertyName, object value);
    }
}
