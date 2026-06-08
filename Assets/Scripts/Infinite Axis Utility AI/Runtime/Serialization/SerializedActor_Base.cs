using UnityEngine;

namespace Stirge.UtilityAI.Serialization
{
    using Core;
    using Enemy;

    public abstract class SerializedActor_Base : ScriptableObject
    {
        public abstract Actor CreateActor(UtilityEnemy enemy);
    }
}
