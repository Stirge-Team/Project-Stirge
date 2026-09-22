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

        private static bool s_brainDebugInfoFoldout;
        private static GUIStyle s_centredLabel;
        private static bool s_centredLabelIsInitialised;

        private void OnEnable()
        {
            m_enemy = (UtilityEnemy)target;
            s_brainDebugInfoFoldout = true;
            s_centredLabelIsInitialised = false;
        }

        public override bool RequiresConstantRepaint()
        {
            return s_brainDebugInfoFoldout && m_enemy.Brain != null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EGL.Separator();

            s_brainDebugInfoFoldout = EGL.BeginFoldoutHeaderGroup(s_brainDebugInfoFoldout, new GUIContent("Utility Brain Debug Info"), EditorStyles.foldoutHeader);
            if (s_brainDebugInfoFoldout)
            {
                if (m_enemy.Brain == null)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EGL.TextField("No Utility Brain exists!");
                    }
                    return;
                }

                if (!s_centredLabelIsInitialised)
                {
                    s_centredLabel = new(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                    s_centredLabelIsInitialised = true;
                }

                EGL.BeginVertical(GUI.skin.textArea);

                BrainDebugInfo info = new();
                m_enemy.Brain.GetBrainDebugInfo(ref info);

                EGL.BeginHorizontal();
                EGL.LabelField("Current Action");
                string currentActionText = info.currentActionIndex != -1 ? info.actions[info.currentActionIndex].displayName : "null";
                using (new EditorGUI.DisabledScope(true))
                    EGL.TextField(currentActionText);
                EGL.EndHorizontal();

                EGL.BeginHorizontal();
                EGL.LabelField("Current Movement Goal");
                string currentMovementGoalText = info.currentMovementGoalIndex != -1 ? info.movementGoals[info.currentMovementGoalIndex].displayName : "null";
                using (new EditorGUI.DisabledScope(true))
                    EGL.TextField(currentMovementGoalText);
                EGL.EndHorizontal();

                EGL.BeginHorizontal();
                EGL.LabelField("Time until Evaluates Actions");
                using (new EditorGUI.DisabledScope(true))
                    EGL.TextField(info.actionTimer.ToString());
                EGL.EndHorizontal();

                EGL.BeginHorizontal();
                EGL.LabelField("Time until Evaluates MovementGoals");
                using (new EditorGUI.DisabledScope(true))
                    EGL.TextField(info.movementGoalTimer.ToString());
                EGL.EndHorizontal();

                EGL.LabelField("Action Scores");

                // sort actions by score
                int actionCount = info.actions.Length;
                Action[] sortedActions = new Action[actionCount];
                float[] sortedActionScores = new float[actionCount];
                Array.Copy(info.actions, sortedActions, actionCount);
                Array.Copy(info.actionScores, sortedActionScores, actionCount);
                Array.Sort(sortedActionScores, sortedActions, new DescendingFloatComparer());

                EditorGUI.indentLevel++;
                bool dividerDrawn = false;
                for (int i = 0; i < actionCount; i++)
                {
                    Action currentAction = sortedActions[i];
                    float currentActionScore = sortedActionScores[i];

                    // check if the divider needs to be drawn
                    if (currentActionScore <= 0 && !dividerDrawn)
                    {
                        EGL.LabelField("---------------Invalid---------------", s_centredLabel);
                        dividerDrawn = true;
                    }

                    EGL.BeginHorizontal();
                    EGL.LabelField($"{i + 1}.", GUILayout.MaxWidth(30f));
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EGL.TextField(currentAction.displayName);
                        EGL.TextField(currentActionScore.ToString());
                    }
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
                dividerDrawn = false;
                for (int i = 0; i < movementGoalCount; i++)
                {
                    MovementGoal currentMovementGoal = sortedMovementGoals[i];
                    float currentMovementGoalScore = sortedMovementGoalScores[i];

                    // check if the divider needs to be drawn
                    if (currentMovementGoalScore <= 0 && !dividerDrawn)
                    {
                        EGL.LabelField("---------------Invalid---------------", s_centredLabel);
                        dividerDrawn = true;
                    }

                    EGL.BeginHorizontal();
                    EGL.LabelField($"{i + 1}.", GUILayout.MaxWidth(30f));
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EGL.TextField(currentMovementGoal.displayName);
                        EGL.TextField(currentMovementGoalScore.ToString());
                    }
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
    }
}
