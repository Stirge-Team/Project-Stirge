using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    public abstract class SerializedAttackData_Base : ScriptableObject
    {
        public abstract AttackData CreateAttackData();
    }
}
