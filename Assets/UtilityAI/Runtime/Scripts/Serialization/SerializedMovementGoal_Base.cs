using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedMovementGoal_Base : ScriptableObject
    {
        public abstract Type movementGoalType { get; }

        public abstract MovementGoal CreateRuntimeMovementGoal();
    }
}
