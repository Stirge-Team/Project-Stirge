using System;
using UnityEngine;

namespace Stirge.UtilityAI.Serialization
{
    using Builders;

    public abstract class SerializedAction_Base : ScriptableObject
    {
        public abstract Type actionType { get; }

        public abstract void AddAction(ActorBuilder builder);
    }
}
