using System;
using System.Reflection;
using UnityEngine;

namespace Stirge.InfiniteAxis.Blackboard
{
    public interface IBlackboardTable<T>
    {
        void Setup(PropertyInfo[] propertyInfos);
        
        Type valueType { get; }

        int count { get; }

        object GetObjectValue(T target, int index);

        void SetObjectValue(T target, object value, int index);
    }
}
