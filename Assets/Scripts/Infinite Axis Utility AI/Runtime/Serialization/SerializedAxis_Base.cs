using System;
using UnityEngine;

namespace Stirge.UtilityAI.Serialization
{
    using Builders;

    public abstract class SerializedAxis_Base : ScriptableObject
    {
        public abstract Type axisType { get; }

        public abstract void AddAxis(ActorBuilder builder);
    }
}
