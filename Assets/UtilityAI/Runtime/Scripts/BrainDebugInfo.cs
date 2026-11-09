using UnityEngine;

namespace Stirge.UtilityAI
{
    public class BrainDebugInfo
    {
        public Action[] actions;
        public MovementGoal[] movementGoals;

        public float actionTimer;
        public float movementGoalTimer;

        public float[] actionScores;
        public float[] movementGoalScores;

        public int currentActionIndex;
        public int currentMovementGoalIndex;
    }
}
