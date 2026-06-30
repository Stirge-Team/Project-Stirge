using UnityEngine;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI.Serialization
{
    using Core;

    public abstract class SerializedActor_Base : ScriptableObject
    {
        public abstract Actor CreateActor(Blackboard blackboard);
    }
}
