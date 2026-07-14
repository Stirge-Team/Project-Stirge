using UnityEngine;
using System;
using System.Reflection;

namespace Stirge.UtilityAI.Core
{
    public class AxisDelegate<T>
    {
        public AxisDelegate(BlackboardPropertyName propertyName)
        {
            UtilityEnemy.CachedPropertyInfosLookup.TryGetValue(propertyName.Hash, out PropertyInfo propertyInfo);
            m_getPropertyDelegate = (Func<UtilityEnemy, T>)Delegate.CreateDelegate(
                typeof(Func<UtilityEnemy, T>),
                propertyInfo.GetGetMethod()!);
        }
        
        private Func<UtilityEnemy, T> m_getPropertyDelegate;

        public T GetValue(UtilityEnemy enemy)
        {
            return m_getPropertyDelegate(enemy);
        }
    }
}
