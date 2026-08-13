using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stirge.InfiniteAxis
{
    using Blackboard;

    public class EnemyBlackboard : GenericBlackboard<UtilityEnemy>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private new static void Setup()
        {
            GenericBlackboard<UtilityEnemy>.Setup();
        }

        public EnemyBlackboard(UtilityEnemy target) : base(target) { }
    }
}
