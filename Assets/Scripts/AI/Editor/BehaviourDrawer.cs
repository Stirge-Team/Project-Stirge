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
            SetLabelTextToTypeName(label);
            string typeName = label.text;

            EditorGUI.BeginProperty(m_position, label, m_property);

            Rect foldoutRect = GetNewRect();
            m_property.isExpanded = EditorGUI.Foldout(foldoutRect, m_property.isExpanded, label);
            if (m_property.isExpanded)
            {
                switch (typeName)
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
                        DrawPropertyField("m_attackData");
                        break;
                    case nameof(UpdateLookSpeedBehaviour):
                        DrawPropertyField("m_newDegreesDelta");
                        break;
                    case nameof(EnterPhysicsBehaviour):
                        DrawPropertyField("m_newMode");
                        DrawPropertyField("m_returnToOldModeOnExit");
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
                SetLabelTextToTypeName(label);
                string typeName = label.text;

                switch (typeName)
                {
                    case nameof(AirJuggleBehaviour):
                    case nameof(KnockbackBehaviour):
                    case nameof(LookAtTargetBehaviour):
                    case nameof(MoveToTargetBehaviour):
                    case nameof(UpdateLookSpeedBehaviour):
                    case nameof(AttackingBehaviour):
                        totalLines++;
                        break;
                    case nameof(EnterPhysicsBehaviour):
                        totalLines += 2;
                        break;
                    case nameof(PhysicsBehaviour):
                        totalLines += 3;
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
