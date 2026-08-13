using System.Reflection;
using UnityEngine;

namespace Stirge.InfiniteAxis.Blackboard
{
    public interface IGenericBlackboard
    {
        public static PropertyInfo[] PublicCachedPropertyInfosArray { get; private set; }
    }
}
