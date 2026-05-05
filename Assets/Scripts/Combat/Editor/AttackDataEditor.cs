using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

using EG = UnityEditor.EditorGUI;
using EGL = UnityEditor.EditorGUILayout;
using GL = UnityEngine.GUILayout;

namespace Stirge.Combat.Attacks
{
    using Tools;

    [CustomEditor(typeof(AttackData))]
    public class AttackDataEditor : EasyCustomEditor
    {
        #region Names
        public static string[] AttackNodeNames
        {
            get
            {
                if (attackNodeNames == null || attackNodeNames.Length == 0)
                {
                    PopulateAttackNodeNames();
                }
                return attackNodeNames;
            }
        }
        
        private static string[] attackNodeNames;
        #endregion

        private int m_selectedAttackNode;

        protected override void OnEnableThis()
        {
            PopulateAttackNodeNames();
        }

        protected override void OnGUI()
        {
            SerializedProperty rootProp = serializedObject.FindProperty("m_root");
            if (rootProp.managedReferenceValue == null)
            {
                EG.BeginDisabledGroup(true);
                EGL.TextField("No root AttackNode. Try adding one.");
                EG.EndDisabledGroup();
            }
            else
            {
                EGL.PropertyField(rootProp);
            }

            // add new root section
            if (rootProp.managedReferenceValue == null)
            {
                // select AttackNode popup
                m_selectedAttackNode = EGL.Popup(m_selectedAttackNode, AttackNodeNames);
                
                // create new AttackNode button
                if (GL.Button("Add new " + AttackNodeNames[m_selectedAttackNode]))
                {
                    AttackNode newAttackNode = System.Activator.CreateInstance(AttackNode.AttackNodeTypes[m_selectedAttackNode]) as AttackNode;
                    rootProp.managedReferenceValue = newAttackNode;
                    HasChanged();
                }
            }
            // remove existing root section
            else
            {
                EG.BeginDisabledGroup(true);
                EGL.TextArea("Warning! Deleting the root AttackNode will delete ANY and ALL attached AttackNodes.", new GUIStyle(EditorStyles.textArea) { wordWrap = true, alignment = TextAnchor.MiddleCenter });
                EG.EndDisabledGroup();
                if (GL.Button("Remove root AttackNode."))
                {
                    rootProp.managedReferenceValue = null;
                }
            }
        }

        private static void PopulateAttackNodeNames()
        {
            // get all the AttackNodes from the static list and change them from CamelCase to English
            attackNodeNames = AttackNode.AttackNodeTypes.Select(t => t.Name).ToArray();
            for (int i = 0; i < attackNodeNames.Length; i++)
            {
                // turns camel case into separate words (looks nice)
                attackNodeNames[i] = Regex.Replace(Regex.Replace(attackNodeNames[i], @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
        }
    }
}
