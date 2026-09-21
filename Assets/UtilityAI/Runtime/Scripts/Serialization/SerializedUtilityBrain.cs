using System.Linq;
using UnityEngine;

namespace Stirge.UtilityAI
{
    [CreateAssetMenu(menuName = "Utility AI/Utility Brain", fileName = "New Brain", order = 448)]
    public class SerializedUtilityBrain : ScriptableObject
    {
        [SerializeField] private SerializedAction[] m_serializedActions;
        [SerializeField] private SerializedMovementGoal_Base[] m_serializedMovementGoals;

        public UtilityBrain CreateRuntimeBrain()
        {
            int actionCount = m_serializedActions.Length;
            Action[] actions = new Action[actionCount];
            for (int i = 0; i < actionCount; i++)
            {
                actions[i] = m_serializedActions[i].CreateRuntimeAction();
            }

            int movementGoalCount = m_serializedMovementGoals.Length;
            MovementGoal[] movementGoals = new MovementGoal[movementGoalCount];
            for (int i = 0; i < movementGoalCount; i++)
            {
                movementGoals[i] = m_serializedMovementGoals[i].CreateRuntimeMovementGoal();
            }

            return UtilityBrain.Create(actions, movementGoals);
        }
    }
}
