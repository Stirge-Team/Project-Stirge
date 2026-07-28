using System.Collections.Generic;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    public interface IDecoratorNode
    {
        public void AddAttackNode(AttackNode attackNode);
    }
}
