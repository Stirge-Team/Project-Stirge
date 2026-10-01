using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public abstract class SerializedMovementGoal_Base : ScriptableObject
    {
        [SerializeField, Range(0f, 5f)] protected float m_scoreScaling = 1f;
        [SerializeField] protected float m_duration = 1f;
        [SerializeField] protected string m_displayName;
        [SerializeField] protected SerializedCondition[] m_conditions =  new SerializedCondition[0];
        [SerializeField] protected SerializedScoringMethod_Base[] m_scoringMethods = new SerializedScoringMethod_Base[0];

        public abstract Type MovementGoalType { get; }

        protected ICondition[] CreateRuntimeConditions(MovementGoal movementGoal)
        {
            int conditionCount = m_conditions.Length;
            ICondition[] conditions = new ICondition[conditionCount];
            for (int i = 0; i < conditionCount; i++)
            {
                conditions[i] = m_conditions[i].CreateRuntimeCondition(movementGoal);
            }
            return conditions;
        }
        protected IScoringMethod[] CreateRuntimeScoringMethods(MovementGoal movementGoal)
        {
            int scoringMethodCount = m_scoringMethods.Length;
            IScoringMethod[] scoringMethods = new IScoringMethod[scoringMethodCount];
            for (int i = 0; i < scoringMethodCount; i++)
            {
                scoringMethods[i] = m_scoringMethods[i].CreateRuntimeScoringMethod(movementGoal);
            }
            return scoringMethods;
        }

        public abstract MovementGoal CreateRuntimeMovementGoal();
    }
}
