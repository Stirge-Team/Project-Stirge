using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

using EGL = UnityEditor.EditorGUILayout;

namespace Stirge.UtilityAI.CustomEditors
{
    [CustomEditor(typeof(UtilityEnemy))]
    public class UtilityEnemyEditor : Editor
    {
        private UtilityEnemy m_enemy;
        private bool m_enemyHasBrain;

        private static bool s_brainDebugInfoFoldout;

        private const string NUM_DISPLAY_FORMAT = "0.###";

        private void OnEnable()
        {
            m_enemy = (UtilityEnemy)target;
            s_brainDebugInfoFoldout = true;
            m_enemyHasBrain = false;
        }

        public override bool RequiresConstantRepaint()
        {
            return s_brainDebugInfoFoldout && m_enemyHasBrain;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EGL.Separator();

            s_brainDebugInfoFoldout = EGL.BeginFoldoutHeaderGroup(s_brainDebugInfoFoldout, new GUIContent("Utility Brain Debug Info"), EditorStyles.foldoutHeader);
            if (s_brainDebugInfoFoldout)
            {
                if (!m_enemy.TryGetBrainDebugInfo(out BrainDebugInfo info))
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EGL.TextField("No Utility Brain exists!");
                    }
                    m_enemyHasBrain = false;
                    return;
                }
                else
                {
                    m_enemyHasBrain = true;
                }

                EGL.BeginVertical(GUI.skin.textArea);

                EGL.BeginHorizontal();
                EGL.LabelField("Current Action");
                bool hasCurrentAction = info.currentActionIndex != -1;
                string currentActionText = hasCurrentAction ? info.actions[info.currentActionIndex].DisplayName : "null";
                DrawDisabledText(!hasCurrentAction, currentActionText);
                EGL.EndHorizontal();

                EGL.BeginHorizontal();
                EGL.LabelField("Current Movement Goal");
                bool hasCurrentMovementGoal = info.currentMovementGoalIndex != -1;
                string currentMovementGoalText = hasCurrentMovementGoal ? info.movementGoals[info.currentMovementGoalIndex].displayName : "null";
                DrawDisabledText(!hasCurrentMovementGoal, currentMovementGoalText);
                EGL.EndHorizontal();

                EGL.BeginHorizontal();
                EGL.LabelField("Time until Evaluates Actions");
                DrawDisabledText(info.actionTimer <= 0, Mathf.Max(0, info.actionTimer).ToString(NUM_DISPLAY_FORMAT));
                EGL.EndHorizontal();

                EGL.BeginHorizontal();
                EGL.LabelField("Time until Evaluates MovementGoals");
                DrawDisabledText(info.movementGoalTimer <= 0, Mathf.Max(0, info.movementGoalTimer).ToString(NUM_DISPLAY_FORMAT));
                EGL.EndHorizontal();

                EGL.LabelField("Action Scores");

                // sort actions by score descending
                int actionCount = info.actions.Length;
                Action[] sortedActions = new Action[actionCount];
                float[] sortedActionScores = new float[actionCount];
                Array.Copy(info.actions, sortedActions, actionCount);
                Array.Copy(info.actionScores, sortedActionScores, actionCount);
                Array.Sort(sortedActionScores, sortedActions, new DescendingFloatComparer());

                EditorGUI.indentLevel++;
                for (int i = 0; i < actionCount; i++)
                {
                    Action action = sortedActions[i];
                    float actionScore = sortedActionScores[i];
                    bool isInvalid = actionScore <= 0;

                    EGL.BeginHorizontal();
                    EGL.LabelField($"{i + 1}.", GUILayout.MaxWidth(30f));
                    DrawDisabledText(isInvalid, action.DisplayName, Mathf.Max(0, actionScore).ToString(NUM_DISPLAY_FORMAT));
                    EGL.EndHorizontal();
                }
                EditorGUI.indentLevel--;

                EGL.LabelField("Movement Goal Scores");

                // sort Movement Goals by Score
                int movementGoalCount = info.movementGoals.Length;
                MovementGoal[] sortedMovementGoals = new MovementGoal[movementGoalCount];
                float[] sortedMovementGoalScores = new float[movementGoalCount];
                Array.Copy(info.movementGoals, sortedMovementGoals, movementGoalCount);
                Array.Copy(info.movementGoalScores, sortedMovementGoalScores, movementGoalCount);
                Array.Sort(sortedMovementGoalScores, sortedMovementGoals, new DescendingFloatComparer());

                EditorGUI.indentLevel++;
                for (int i = 0; i < movementGoalCount; i++)
                {
                    MovementGoal movementGoal = sortedMovementGoals[i];
                    float movementGoalScore = sortedMovementGoalScores[i];
                    bool isInvalid = movementGoalScore <= 0;

                    EGL.BeginHorizontal();
                    EGL.LabelField($"{i + 1}.", GUILayout.MaxWidth(30f));
                    DrawDisabledText(isInvalid, movementGoal.displayName, Mathf.Max(0, movementGoalScore).ToString(NUM_DISPLAY_FORMAT));
                    EGL.EndHorizontal();
                }
                EditorGUI.indentLevel--;

                EGL.EndVertical();
            }
            EGL.EndFoldoutHeaderGroup();
        }

        private class DescendingFloatComparer : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                return y.CompareTo(x);
            }
        }

        private static void DrawDisabledText(bool showDisabled, params string[] labels)
        {
            using (new EditorGUI.DisabledScope(showDisabled))
            {
                foreach (string label in labels)
                {
                    EGL.LabelField(label, EditorStyles.textField);
                }
            }
        }
    }
}
