using System.Collections.Generic;
using UnityEngine;

namespace Stirge.UtilityAI
{
    using Combat;

    public class UtilityBrain
    {
        private Action[] m_actions;
        private MovementGoal[] m_movementGoals;

        private float[] m_actionScores;
        private float[] m_movementGoalScores;

        // the index of the current Action that is being performed in m_actions
        private int m_currentActionIndex;
        // the index of the current MovementGoal that is being performed in m_movementGoals
        private int m_currentMovementGoalIndex;

        private float m_minimumActionScore;
        private float m_minimumMovementGoalScore;

        private float m_actionCountdown;
        private float m_movementGoalCountdown;

        // Used to check if the current Action has been performed yet, as we only want an Action to be performed once each time it is selected
        private bool m_actionPerformed;

        // properties
        public Action CurrentAction => m_currentActionIndex == -1 ? null : m_actions[m_currentActionIndex];
        public MovementGoal CurrentMovementGoal => m_currentMovementGoalIndex == -1 ? null : m_movementGoals[m_currentMovementGoalIndex];

        public void Start()
        {
            m_actionScores = new float[m_actions.Length];
            m_movementGoalScores = new float[m_movementGoals.Length];

            m_currentActionIndex = -1;
            m_currentMovementGoalIndex = -1;

            m_actionPerformed = false;
        }

        public void Update(UtilityEnemy user, CombatEntity target)
        {
            EvaluateActions(user, target);
            EvaluateMovementGoals(user, target);

            // Perform Action if not yet performed
            if (!m_actionPerformed && m_currentActionIndex != -1)
            {
                CurrentAction.Perform(user, target);
                m_actionPerformed = true;
            }

            // Perform MovementGoal!
            CurrentMovementGoal?.Perform(user, target);
        }

        private void EvaluateActions(UtilityEnemy user, CombatEntity target)
        {
            // Either update timer or determine a new Action to Perform
            if (m_actionCountdown <= 0f)
            {
                // Evaluate the scores for each Action
                List<int> validIndices = new();
                for (int i = 0, count = m_actions.Length; i < count; i++)
                {
                    m_actionScores[i] = m_actions[i].Evaluate(user, target);
                    if (m_actionScores[i] > m_minimumActionScore)
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
                            m_actionCountdown = m_actions[currentActionIndex].duration;
                            m_actionPerformed = false;
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
                m_actionCountdown -= Time.deltaTime;
            }
        }

        private void EvaluateMovementGoals(UtilityEnemy user, CombatEntity target)
        {
            // Either update timer or determine a new MovementGoal to Perform
            if (m_movementGoalCountdown <= 0f)
            {
                // Evaluate the scores for each MovementGoal
                List<int> validIndices = new();
                for (int i = 0, count = m_movementGoals.Length; i < count; i++)
                {
                    m_movementGoalScores[i] = m_movementGoals[i].Evaluate(user, target);
                    if (m_movementGoalScores[i] > m_minimumMovementGoalScore)
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
                            m_movementGoalCountdown = m_movementGoals[m_currentMovementGoalIndex].duration;
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
                m_movementGoalCountdown -= Time.deltaTime;
            }
        }

        #region Create
        public static UtilityBrain Create(Action[] actions, MovementGoal[] movementGoals, float minimumActionScore, float minimumMovementGoalScore)
        {
            var newBrain = new UtilityBrain()
            {
                m_actions = actions,
                m_movementGoals = movementGoals,
                m_minimumActionScore = minimumActionScore,
                m_minimumMovementGoalScore = minimumMovementGoalScore
            };
            return newBrain;
        }
        #endregion

#if UNITY_EDITOR
        public void GetBrainDebugInfo(ref BrainDebugInfo info)
        {
            info.actions = m_actions;
            info.movementGoals = m_movementGoals;
            info.actionTimer = m_actionCountdown;
            info.movementGoalTimer = m_movementGoalCountdown;
            info.actionScores = m_actionScores;
            info.movementGoalScores = m_movementGoalScores;
            info.currentActionIndex = m_currentActionIndex;
            info.currentMovementGoalIndex = m_currentMovementGoalIndex;
        }
#endif
    }
}
