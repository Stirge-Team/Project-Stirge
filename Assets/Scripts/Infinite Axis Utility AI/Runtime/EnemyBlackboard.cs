using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Blackboard;
    using Stirge.UtilityAI.Demo;

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
