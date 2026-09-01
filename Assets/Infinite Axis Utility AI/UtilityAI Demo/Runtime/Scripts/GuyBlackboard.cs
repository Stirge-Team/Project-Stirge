using UnityEngine;

namespace Stirge.InfiniteAxis.Demo
{
    using GenericBlackboard;

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
