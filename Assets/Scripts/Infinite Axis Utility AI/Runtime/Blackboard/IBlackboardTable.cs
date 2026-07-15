using System;
using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public interface IBlackboardTable
    {
        void Setup(PropertyInfo[] propertyInfos);
        
        Type valueType { get; }

        int count { get; }

        object GetObjectValue(UtilityEnemy enemy, int index);

        void SetObjectValue(UtilityEnemy enemy, object value, int index);
    }
}
