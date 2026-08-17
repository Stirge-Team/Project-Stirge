using UnityEngine;

namespace Stirge.UtilityAI.Demo
{
    using Blackboard;

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
