using Stirge.Combat;
using Stirge.GenericBlackboard;
using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface ICondition
    {
        public void Init(Operation operation, object firstObject, object secondObject, bool firstIsStruct, bool secondIsStruct);
        public void Init(Operation operation, object obj, BlackboardPropertyName propertyName, bool firstIsStruct, bool secondIsStruct);
        public void Init(Operation operation, BlackboardPropertyName propertyName, object obj, bool firstIsStruct, bool secondIsStruct);
        public void Init(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, bool firstIsStruct, bool secondIsStruct);
        public void Setup(Action action);
        public bool Evaluate(CombatEntity user, CombatEntity target);
    }
}
