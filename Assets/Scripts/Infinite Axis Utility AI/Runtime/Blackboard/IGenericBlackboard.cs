using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI.Blackboard
{
    public interface IGenericBlackboard
    {
        public static PropertyInfo[] PublicCachedPropertyInfosArray { get; private set; }
    }
}
