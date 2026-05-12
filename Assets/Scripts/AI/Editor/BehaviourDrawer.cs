using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    using Tools;
    
    [CustomPropertyDrawer(typeof(Behaviour), true)]
    public class BehaviourDrawer : EasyPropertyDrawer
    {
        protected override void DrawGUI(GUIContent label)
        {
            string typeName = m_property.type;
            // format if managed reference aka not FSM
            if (typeName == nameof(FiniteStateMachine))
            {
                label.text = typeName;
            }
            else
            {
                typeName = typeName.Substring(17, typeName.Length - 18);
                
                // if Behaviour, then prompt deletion
                if (typeName == string.Empty)
                    label.text = "Empty, pls delete";
                else
                    label.text = typeName;
            }

            EditorGUI.BeginProperty(m_position, label, m_property);

            Rect foldoutRect = GetNewRect();
            m_property.isExpanded = EditorGUI.Foldout(foldoutRect, m_property.isExpanded, label);
            if (m_property.isExpanded)
            {
                switch (label.text)
                {
                    case nameof(AirJuggleBehaviour):
                    case nameof(KnockbackBehaviour):
                        DrawPropertyField("m_offGroundTime");
                        break;
                    case nameof(PhysicsBehaviour):
                        DrawPropertyField("m_maintainPriorMode");
                        DrawPropertyField("m_enterMode");
                        DrawPropertyField("m_exitMode");
                        break;
                    case nameof(LookAtTargetBehaviour):
                        DrawPropertyField("m_maxDegreesDelta");
                        break;
                    case nameof(MoveToTargetBehaviour):
                        DrawPropertyField("m_speed");
                        break;
                    case nameof(AttackingBehaviour):
                        DrawPropertyField("m_exitState");
                        DrawPropertyField("m_attackData");
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
                // format if managed reference aka not FSM
                if (typeName != nameof(FiniteStateMachine))
                {
                    typeName = typeName.Substring(17, typeName.Length - 18);
                }

                switch (typeName)
                {
                    case nameof(AirJuggleBehaviour):
                    case nameof(KnockbackBehaviour):
                    case nameof(LookAtTargetBehaviour):
                    case nameof(MoveToTargetBehaviour):
                        totalLines++;
                        break;
                    case nameof(PhysicsBehaviour):
                        totalLines += 3;
                        break;
                    case nameof(AttackingBehaviour):
                        totalLines++;
                        totalLines += GetPropertyLineHeight("m_attackData");
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
