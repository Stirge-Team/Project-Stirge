using Stirge.GenericBlackboard;
using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface ICondition
    {
        protected enum ObjectType
        {
            Object,
            Property
        }

        public void Init(Operation operation, object firstObject, object secondObject, Type firstType, Type secondType);
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, Type objType);
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName);
        public void Setup(Action action);
        public bool Evaluate();

        protected object GetFirstObject();
        protected object GetSecondObject();
        protected bool TryGetFirst<T>(out T value);
        protected bool TryGetSecond<T>(out T value);
    }
}
