using System;
using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public interface IBlackboardTable
    {
        void Setup(UtilityEnemy enemy, PropertyInfo[] propertyInfos);
        
        Type valueType { get; }

        object GetObjectValue(int index);

        void SetObjectValue(object value, int index);
    }
}
