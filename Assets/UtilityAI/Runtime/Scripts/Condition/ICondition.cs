using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface ICondition
    {
        public void Init(Operation operation, object firstObject, object secondObject, Type firstType, Type secondType);
        public void Setup(Action action);
        public bool Evaluate();
    }
}
