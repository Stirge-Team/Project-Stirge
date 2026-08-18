using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface ICondition
    {
        public void Init(Operation operation, object firstObject, object secondObject);
        public bool Evaluate();
    }
}
