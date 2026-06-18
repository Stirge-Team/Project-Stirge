using UnityEditor;
using UnityEngine;

namespace Stirge.Combat.Attacks
{
    using Tools;
    
    [CustomPropertyDrawer(typeof(AttackNode), true)]
    public class AttackNodeDrawer : EasyPropertyDrawer
    {
        private int m_selectedAttackNode;

        protected override void DrawGUI(GUIContent label)
        {
            SetLabelTextToTypeName(label);
            string typeName = label.text;

            EditorGUI.BeginProperty(m_position, label, m_property);
            DrawLabelHeader(label);
            if (m_property.isExpanded)
            {
                // check for abstract types
                var obj = m_property.managedReferenceValue;
                if (obj is MoveNode)
                {
                    DrawPropertyField("m_localOffset");
                    m_totalLines += GetPropertyLineHeight("m_localOffset") - 1;
                    DrawPropertyField("m_stoppingDistance");
                    DrawPropertyField("m_considerYPosition");
                }

                switch (typeName)
                {
                    #region Nodes
                    case nameof(AnimationNode):
                        DrawPropertyField("m_animationStateName");
                        DrawPropertyField("m_animationClip");
                        DrawPropertyField("m_speed");
                        DrawPropertyField("m_hasRootMotion");
                        break;
                    case nameof(ApproachTargetNode):
                        DrawPropertyField("m_stoppingDistance");
                        DrawPropertyField("m_useInitialPosition");
                        DrawPropertyField("m_speed");
                        break;
                    case nameof(TranslateNode):
                        DrawPropertyField("m_translation");
                        DrawPropertyField("m_isLocalTranslation");
                        DrawPropertyField("m_time");
                        break;
                    case nameof(DelayNode):
                        DrawPropertyField("m_delay");
                        break;
                    case nameof(TimedMoveNode):
                        DrawPropertyField("m_time");
                        break;
                    case nameof(CurveMoveNode):
                        DrawPropertyField("m_curve");
                        break;
                    case nameof(SpeedMoveNode):
                        DrawPropertyField("m_speed");
                        break;
                    case nameof(AccelerateMoveNode):
                        DrawPropertyField("m_acceleration");
                        using (new EditorGUI.DisabledScope(true))
                            EditorGUI.TextArea(GetNewRect(), "If Max Speed is less than or equal to 0, it will be treated as infinite.");
                        DrawPropertyField("m_maxSpeed");
                        break;
                    #endregion
                    #region Decorators
                    case nameof(ChanceNode):
                        DrawPropertyField("m_chance");
                        DrawPropertyField("m_node");
                        m_totalLines += GetPropertyLineHeight("m_node") - 1;

                        // add Attack Node to array button
                        // select AttackNode popup
                        m_selectedAttackNode = EditorGUI.Popup(GetNewRect(), m_selectedAttackNode, AttackDataEditor.AttackNodeNames);

                        // create new AttackNode button
                        if (GUI.Button(GetNewRect(), "Set Chance node to " + AttackDataEditor.AttackNodeNames[m_selectedAttackNode]))
                        {
                            AttackNode newAttackNode = System.Activator.CreateInstance(AttackNode.AttackNodeTypes[m_selectedAttackNode]) as AttackNode;
                            SerializedProperty nodeProp = FindPropertyRelative("m_node");
                            nodeProp.managedReferenceValue = newAttackNode;
                        }

                        break;
                    case nameof(SelectAttackNode):
                    case nameof(SequenceAttackNode):
                    case nameof(SimultaneousAttackNode):
                        DrawPropertyField("m_significantAttackNodeIndex");
                        DrawPropertyField("m_nodes");

                        // add Attack Node to array button
                        // select AttackNode popup
                        m_selectedAttackNode = EditorGUI.Popup(GetNewRect(), m_selectedAttackNode, AttackDataEditor.AttackNodeNames);

                        // create new AttackNode button
                        if (GUI.Button(GetNewRect(), "Add to list " + AttackDataEditor.AttackNodeNames[m_selectedAttackNode]))
                        {
                            AttackNode newAttackNode = System.Activator.CreateInstance(AttackNode.AttackNodeTypes[m_selectedAttackNode]) as AttackNode;
                            SerializedProperty nodesProp = FindPropertyRelative("m_nodes");
                            nodesProp.arraySize++;
                            SerializedProperty newAttackNodeProp = nodesProp.GetArrayElementAtIndex(nodesProp.arraySize - 1);
                            newAttackNodeProp.managedReferenceValue = newAttackNode;
                            nodesProp.isExpanded = true;
                            newAttackNodeProp.isExpanded = true;
                        }
                        break;
                    #endregion
                }
            }

            EditorGUI.EndProperty();
        }

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1; // for foldout/label

            if (m_property.isExpanded)
            {
                SetLabelTextToTypeName(label);
                string typeName = label.text;

                var obj = m_property.managedReferenceValue;
                if (obj is MoveNode)
                {
                    totalLines++; // considerY bool
                    totalLines += GetPropertyLineHeight("m_stoppingDistance");
                    totalLines += GetPropertyLineHeight("m_localOffset");
                }

                switch (typeName)
                {
                    #region Nodes
                    case nameof(AnimationNode):
                        totalLines += GetPropertyLineHeight("m_speed");
                        totalLines += 3;
                        break;
                    case nameof(ApproachTargetNode):
                        totalLines += GetPropertyLineHeight("m_stoppingDistance");
                        totalLines += GetPropertyLineHeight("m_speed");
                        totalLines++;
                        break;
                    case nameof(TranslateNode):
                        totalLines += GetPropertyLineHeight("m_time");
                        totalLines += 2;
                        break;
                    case nameof(DelayNode):
                        totalLines += GetPropertyLineHeight("m_delay");
                        break;
                    case nameof(TimedMoveNode):
                        totalLines += GetPropertyLineHeight("m_time");
                        break;
                    case nameof(CurveMoveNode):
                        totalLines += GetPropertyLineHeight("m_curve");
                        break;
                    case nameof(SpeedMoveNode):
                        totalLines += GetPropertyLineHeight("m_speed");
                        break;
                    case nameof(AccelerateMoveNode):
                        totalLines++; // for tooltip
                        totalLines += GetPropertyLineHeight("m_acceleration");
                        totalLines += GetPropertyLineHeight("m_maxSpeed");
                        break;
                    #endregion
                    #region Decorators
                    case nameof(ChanceNode):
                        totalLines += GetPropertyLineHeight("m_node");
                        totalLines += 3; // for chance, popup, and add Node button
                        break;
                    case nameof(SelectAttackNode):
                    case nameof(SequenceAttackNode):
                        totalLines += GetPropertyLineHeight("m_nodes");
                        totalLines += 3; // for popup and add button
                        break;
                    case nameof(SimultaneousAttackNode):
                        totalLines += GetPropertyLineHeight("m_nodes");
                        totalLines += 3; // for popup and add button, and for Significant Index
                        break;
                    #endregion
                }
            }
            
            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
