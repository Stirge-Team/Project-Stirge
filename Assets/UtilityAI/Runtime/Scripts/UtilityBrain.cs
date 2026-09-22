using System.Collections.Generic;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    public class UtilityBrain
    {
        private Action[] m_actions;
        private MovementGoal[] m_movementGoals;

        private float m_actionTimer;
        private float m_movementGoalTimer;

        private float[] m_actionScores;
        private float[] m_movementGoalScores;

        private int m_currentActionIndex;
        private int m_currentMovementGoalIndex;

        // properties
        public Action CurrentAction => m_currentActionIndex == -1 ? null : m_actions[m_currentActionIndex];
        public MovementGoal CurrentMovementGoal => m_currentMovementGoalIndex == -1 ? null : m_movementGoals[m_currentMovementGoalIndex];

        public void Start()
        {
            m_actionScores = new float[m_actions.Length];
            m_movementGoalScores = new float[m_movementGoals.Length];

            m_currentActionIndex = -1;
            m_currentMovementGoalIndex = -1;
        }

        public void Update(UtilityEnemy user, CombatEntity target)
        {
            EvaluateActions(user, target);
            EvaluateMovementGoals(user, target);

            // Perform Action!
            CurrentAction?.Perform(user, target);

            // Perform MovementGoal!
            CurrentMovementGoal?.Perform(user, target);
        }

        private void EvaluateActions(UtilityEnemy user, CombatEntity target)
        {
            // Either update timer or determine a new Action to Perform
            if (m_actionTimer <= 0f)
            {
                // Evaluate the scores for each Action
                List<int> validIndices = new();
                for (int i = 0, count = m_actions.Length; i < count; i++)
                {
                    if ((m_actionScores[i] = m_actions[i].Evaluate(user, target)) > 0)
                        validIndices.Add(i);
                }

                // If there are valid Actions
                if (validIndices.Count > 0)
                {
                    // Randomly decide which Action to Perform, not including invalid Actions
                    float totalScore = 0;
                    for (int i = 0, count = validIndices.Count; i < count; i++)
                    {
                        totalScore += m_actionScores[validIndices[i]];
                    }

                    float targetScore = Random.value * totalScore;
                    float runningScore = 0;
                    for (int i = 0, count = validIndices.Count; i < count; i++)
                    {
                        int currentActionIndex = validIndices[i];
                        // update running total
                        runningScore += m_actionScores[currentActionIndex];

                        // if running total breaches our targetScore, we've landed on our target
                        if (runningScore > targetScore)
                        {
                            // Even if the new Action is the same as the previous, reset the duration
                            m_currentActionIndex = currentActionIndex;
                            m_actionTimer = m_actions[currentActionIndex].duration;
                            break;
                        }
                    }
                }
                // If there are no valid Actions
                else
                {
                    m_currentActionIndex = -1;
                }
            }
            else
            {
                m_actionTimer -= Time.deltaTime;
            }
        }

        private void EvaluateMovementGoals(UtilityEnemy user, CombatEntity target)
        {
            // Either update timer or determine a new MovementGoal to Perform
            if (m_movementGoalTimer <= 0f)
            {
                // Evaluate the scores for each MovementGoal
                List<int> validIndices = new();
                for (int i = 0, count = m_movementGoals.Length; i < count; i++)
                {
                    if ((m_movementGoalScores[i] = m_movementGoals[i].Evaluate(user, target)) > 0)
                        validIndices.Add(i);
                }

                // If there are valid MovementGoals
                if (validIndices.Count > 0)
                {
                    // Randomly decide which MovementGoal to Perform, not including invalid MovementGoals
                    float totalScore = 0;
                    for (int i = 0, count = validIndices.Count; i < count; i++)
                    {
                        totalScore += m_movementGoalScores[validIndices[i]];
                    }

                    float targetScore = Random.value * totalScore;
                    float runningScore = 0;
                    for (int i = 0, count = validIndices.Count; i < count; i++)
                    {
                        int currentMovementGoalIndex = validIndices[i];
                        // update running total
                        runningScore += m_movementGoalScores[currentMovementGoalIndex];

                        // if running total breaches our targetScore, we've landed on our target
                        if (runningScore > targetScore)
                        {
                            // If the new MovementGoal is different, Reset it before it begins Performing
                            if (currentMovementGoalIndex != m_currentMovementGoalIndex)
                            {
                                m_movementGoals[currentMovementGoalIndex].Reset();
                            }
                            
                            // Even if the new MovementGoal is the same as the previous, reset the duration
                            m_currentMovementGoalIndex = currentMovementGoalIndex;
                            m_movementGoalTimer = m_movementGoals[m_currentMovementGoalIndex].duration;
                            break;
                        }
                    }
                }
                // If there are no valid MovementGoals
                else
                {
                    m_currentMovementGoalIndex = -1;
                }
            }
            else
            {
                m_movementGoalTimer -= Time.deltaTime;
            }
        }

        #region Create
        public static UtilityBrain Create(Action[] actions, MovementGoal[] movementGoals)
        {
            var newBrain = new UtilityBrain()
            {
                m_actions = actions,
                m_movementGoals = movementGoals
            };
            return newBrain;
        }
        #endregion

#if UNITY_EDITOR
        public void GetBrainDebugInfo(ref BrainDebugInfo info)
        {
            info.actions = m_actions;
            info.movementGoals = m_movementGoals;
            info.actionTimer = m_actionTimer;
            info.movementGoalTimer = m_movementGoalTimer;
            info.actionScores = m_actionScores;
            info.movementGoalScores = m_movementGoalScores;
            info.currentActionIndex = m_currentActionIndex;
            info.currentMovementGoalIndex = m_currentMovementGoalIndex;
        }
#endif
    }
}
