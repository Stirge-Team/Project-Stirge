using UnityEngine;

namespace Stirge.UtilityAI
{
    using GenericBlackboard;

    public class UtilityEnemyBlackboard
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            GenericBlackboard<UtilityEnemy>.Setup();
        }
    }
}
