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
            string typeName = m_property.type;
            typeName = typeName.Substring(17, typeName.Length - 18);

            // if AttackNode, then prompt deletion
            if (typeName == string.Empty)
                label.text = "Empty, pls delete";
            else
                label.text = typeName;

            EditorGUI.BeginProperty(m_position, label, m_property);
            m_property.isExpanded = EditorGUI.Foldout(GetNewRect(), m_property.isExpanded, label);
            if (m_property.isExpanded)
            {
                switch (typeName)
                {
                    case nameof(AnimationNode):
                        DrawPropertyField("m_animation");
                        DrawPropertyField("m_speed");
                        DrawPropertyField("m_hasRootMotion");
                        break;
                    case nameof(ApproachTargetNode):
                        DrawPropertyField("m_stoppingDistance");
                        DrawPropertyField("m_useInitialPosition");
                        DrawPropertyField("m_time");
                        break;
                    case nameof(TranslateNode):
                        DrawPropertyField("m_translation");
                        DrawPropertyField("m_isLocalTranslation");
                        DrawPropertyField("m_time");
                        break;
                    case nameof(DelayNode):
                        DrawPropertyField("m_delay");
                        break;
                    case nameof(ChanceNode):
                        DrawPropertyField("m_chance");
                        DrawPropertyField("m_node");

                        // add Attack Node to array button
                        // select AttackNode popup
                        m_selectedAttackNode = EditorGUI.Popup(GetNewRect(), m_selectedAttackNode, AttackDataEditor.AttackNodeNames);

                        // create new AttackNode button
                        if (GUI.Button(GetNewRect(), "Add new " + AttackDataEditor.AttackNodeNames[m_selectedAttackNode]))
                        {
                            AttackNode newAttackNode = System.Activator.CreateInstance(AttackNode.AttackNodeTypes[m_selectedAttackNode]) as AttackNode;
                            SerializedProperty nodeProp = FindPropertyRelative("m_node");
                            nodeProp.managedReferenceValue = newAttackNode;
                        }

                        break;
                    case nameof(SelectAttackNode):
                    case nameof(SequenceAttackNode):
                        DrawPropertyField("m_nodes");

                        // add Attack Node to array button
                        // select AttackNode popup
                        m_selectedAttackNode = EditorGUI.Popup(GetNewRect(), m_selectedAttackNode, AttackDataEditor.AttackNodeNames);

                        // create new AttackNode button
                        if (GUI.Button(GetNewRect(), "Set new " + AttackDataEditor.AttackNodeNames[m_selectedAttackNode]))
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
                }
            }

            EditorGUI.EndProperty();
        }

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (m_property.isExpanded)
            {
                string typeName = m_property.type;
                typeName = typeName.Substring(17, typeName.Length - 18);

                switch (typeName)
                {
                    case nameof(AnimationNode):
                        totalLines += GetPropertyLineHeight("m_speed");
                        totalLines += 2;
                        break;
                    case nameof(ApproachTargetNode):
                        totalLines += GetPropertyLineHeight("m_stoppingDistance");
                        totalLines += GetPropertyLineHeight("m_time");
                        totalLines++;
                        break;
                    case nameof(TranslateNode):
                        totalLines += GetPropertyLineHeight("m_time");
                        totalLines += 2;
                        break;
                    case nameof(DelayNode):
                        totalLines++;
                        break;
                    case nameof(ChanceNode):
                        totalLines += GetPropertyLineHeight("m_node");
                        totalLines += 3; // for chance, popup, and add Node button
                        break;
                    case nameof(SelectAttackNode):
                    case nameof(SequenceAttackNode):
                        totalLines += GetPropertyLineHeight("m_nodes");
                        totalLines += 2; // for popup and add button
                        break;
                }
            }
            
            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
