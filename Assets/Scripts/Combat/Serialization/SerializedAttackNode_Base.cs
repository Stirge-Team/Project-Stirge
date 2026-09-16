using System;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    public abstract class SerializedAttackNode_Base : ScriptableObject
    {
        /// <summary>
        /// Type of serialized <see cref="AttackNode"/>
        /// </summary>
        public abstract Type attackNodeType { get; }

        public abstract void AddAttackNode(AttackDataBuilder builder);
    }
}
