using UnityEngine;

namespace Stirge.UtilityAI.Serialization
{
    using Blackboard;
    using Core;

    public abstract class SerializedActor_Base : ScriptableObject
    {
        public abstract Actor CreateActor(GenericBlackboard_Base enemy);
    }
}
