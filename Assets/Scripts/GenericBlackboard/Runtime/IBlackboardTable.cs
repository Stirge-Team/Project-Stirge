using System;
using System.Reflection;
using UnityEngine;

namespace Stirge.GenericBlackboard
{
    public interface IBlackboardTable<TBase>
    {
        void Setup(PropertyInfo[] propertyInfos);
        
        Type valueType { get; }

        int count { get; }

        object GetObjectValue(TBase target, int index);

        void SetObjectValue(TBase target, object value, int index);
    }
}
