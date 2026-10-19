using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;
    using GenericBlackboard;

    public abstract class GenericCondition_Base : ICondition
    {        
        public abstract bool Evaluate(CombatEntity user, CombatEntity target);

        public abstract void Setup<T>(T scorable) where T : class, IScorable;
        public abstract void Setup(Operation operation, object firstObject, object secondObject);
        public abstract void Setup(Operation operation, object obj, BlackboardPropertyName propertyName, ConditionPropertyTarget propertyTarget);
        public abstract void Setup(Operation operation, BlackboardPropertyName propertyName, object obj, ConditionPropertyTarget propertyTarget);
        public abstract void Setup(Operation operation, BlackboardPropertyName firstPropertyName, BlackboardPropertyName secondPropertyName, ConditionPropertyTarget firstPropertyTarget, ConditionPropertyTarget secondPropertyTarget);
    }
}
