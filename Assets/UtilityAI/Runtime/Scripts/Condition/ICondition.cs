using Stirge.GenericBlackboard;
using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface ICondition
    {
        public void Init(Operation operation, object firstObject, object secondObject, Type firstType, Type secondType);
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, Type firstType, Type secondType);
        public void Init(Operation operation, BlackboardPropertyName propertyName, object obj, Type firstType, Type secondType);
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, Type firstType, Type secondType);
        public void Setup(Action action);
        public bool Evaluate();

        public object FirstObject { get; }
        public object SecondObject { get; }
    }
}
