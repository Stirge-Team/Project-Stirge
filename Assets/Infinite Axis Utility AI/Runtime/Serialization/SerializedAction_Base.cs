using System;
using UnityEngine;

namespace Stirge.InfiniteAxis.Serialization
{
    using Builders;

    public abstract class SerializedAction_Base : ScriptableObject
    {
        public abstract Type actionType { get; }

        public abstract void AddAction(ActorBuilder builder);
    }
}
