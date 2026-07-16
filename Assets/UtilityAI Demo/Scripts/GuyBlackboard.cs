using Stirge.UtilityAI.Blackboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stirge.UtilityAI.Demo
{
    public class GuyBlackboard : GenericBlackboard<Guy>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private new static void Setup()
        {
            GenericBlackboard<Guy>.Setup();
        }

        public GuyBlackboard(Guy target) : base(target) { }
    }
}
