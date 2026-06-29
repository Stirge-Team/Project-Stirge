using System;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public interface IBlackboardTable
    {
        Type valueType { get; }

        int count { get; }

        object GetObjectValue(int index);

        void SetObjectValue(object value, int index);

        int AddObjectValue(object value);

        void RemoveAt(int index);

        void Clear();
    }
}
