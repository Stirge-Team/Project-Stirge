using UnityEngine;

namespace Stirge.InfiniteAxis.Serialization
{
    using GenericBlackboard;
    using Core;

    public abstract class SerializedActor_Base : ScriptableObject
    {
        public abstract Actor CreateActor();
    }
}
