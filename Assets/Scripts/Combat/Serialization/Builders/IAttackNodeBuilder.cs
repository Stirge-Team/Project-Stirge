using System;
using UnityEngine;

namespace Stirge.Combat.Attacks.Serialization
{
    public interface IAttackNodeBuilder
    {
        Type attackNodeType { get; }

        AttackNode Build();

        bool AreEqual(object[] parameters);
    }
}
