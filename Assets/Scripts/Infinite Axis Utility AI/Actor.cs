using UnityEngine;
using Zor.SimpleBlackboard.Core;

namespace Stirge.UtilityAI
{
    public sealed class Actor
    {
        private readonly Axis[] m_axes;
        private readonly Action[] m_actions;

        private readonly Blackboard m_blackboard;

        private float[] m_axisScores;
        private float[] m_actionScores;

        /// <summary>
        /// m_actionAxisBindings[x] contains an array of indicies of the Axes in m_axes considered by the Action at m_actions[x].
        /// </summary>
        private int[][] m_actionAxisBindings;

        private int m_currentActionIndex;

        public Actor() { }
        public Actor(Axis[] axes, Action[] actions, int[][] actionAxisBindings, Blackboard blackboard)
        {
            m_axes = axes;
            m_actions = actions;
            m_blackboard = blackboard;
            m_actionAxisBindings = actionAxisBindings;

            m_axisScores = new float[m_axes.Length];
            m_actionScores = new float[m_actions.Length];

            for (int i = 0; i < m_axes.Length; i++)
            {
                m_axes[i].SetBlackboard(m_blackboard);
            }
            for (int i = 0; i < m_actions.Length; i++)
            {
                m_actions[i].SetBlackboard(m_blackboard);
            }
        }

        public void Initialise()
        {
            InitialiseAxes();
            InitialiseActions();
            UpdateAxisScores();
            UpdateActionScores();
            m_currentActionIndex = FindBestActionIndex();
            m_actions[m_currentActionIndex].Begin();
        }

        public void Update()
        {
            UpdateAxisScores();
            UpdateActionScores();
            SwitchAction(FindBestActionIndex());
            m_actions[m_currentActionIndex].Update();
        }

        private void InitialiseAxes()
        {
            for (int i = 0; i < m_axes.Length; i++)
            {
                m_axes[i].Initialise();
            }
        }

        private void InitialiseActions()
        {
            for (int i = 0; i < m_actions.Length; i++)
            {
                m_actions[i].Initialise();
            }
        }

        private void UpdateAxisScores()
        {
            for (int i = 0; i < m_axisScores.Length; i++)
            {
                m_axisScores[i] = m_axes[i].ComputeScore();
            }
        }
        
        private void UpdateActionScores()
        {
            for (int i = 0; i < m_actionScores.Length; i++)
            {
                m_actionScores[i] = DetermineActionScore(i);
            }
        }

        private float DetermineActionScore(int actionIndex)
        {
            // Get the indices of the Axes bound to this Action
            int[] scoreIndices = m_actionAxisBindings[actionIndex];

            // Get the product of the scores of each Axis
            float product = 1f;
            int count;
            for (count = 0; count < scoreIndices.Length; count++)
            {
                product *= m_axisScores[scoreIndices[count]];
            }

            // Since all values are 0-1, adding more axes drags score lower, unfairly penalising complex actions
            // Instead return geometric mean, which fairly balances the axes regardless of count
            // and ensures that if any Axis has a score of 0, the final score will also be 0
            if (count <= 1)
                return product;
            else
                return Mathf.Pow(product, 1f / scoreIndices.Length);
        }

        private int FindBestActionIndex()
        {
            int bestActionIndex = 0;
            float bestActionScore = m_actionScores[bestActionIndex];

            for (int i = 1; i < m_actionScores.Length; i++)
            {
                float actionScore = m_actionScores[i];
                if (actionScore > bestActionScore)
                {
                    bestActionIndex = i;
                    bestActionScore = actionScore;
                }
            }

            return bestActionIndex;
        }

        private void SwitchAction(int newActionIndex)
        {
            if (m_currentActionIndex == newActionIndex)
                return;

            m_actions[m_currentActionIndex].End();
            m_currentActionIndex = newActionIndex;
            m_actions[m_currentActionIndex].Begin();
        }
    }
}
