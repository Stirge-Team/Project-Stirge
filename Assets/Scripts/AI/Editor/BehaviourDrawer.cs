using Stirge.Combat;
using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    [CustomPropertyDrawer(typeof(Behaviour), true)]
    public class BehaviourDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int totalLines = 0;

            void DrawPropertyField(string propertyName)
            {
                SerializedProperty propToDraw = property.FindPropertyRelative(propertyName);
                Rect propRect = GetNewRect();
                EditorGUI.PropertyField(propRect, propToDraw);

                if (propToDraw.propertyType == SerializedPropertyType.Float)
                {
                    if (propToDraw.floatValue < 0)
                        propToDraw.floatValue = 0;
                }
            }

            Rect GetNewRect()
            {
                totalLines++;
                return new Rect(position.min.x + EditorGUI.indentLevel * 15f, position.min.y + EditorGUIUtility.singleLineHeight * (totalLines - 1), position.size.x - EditorGUI.indentLevel * 15f, EditorGUIUtility.singleLineHeight);
            }

            string typeName = property.type;
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

            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutRect = GetNewRect();
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
            if (property.isExpanded)
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
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLines = 1; // for foldout

            if (property.isExpanded)
            {
                string typeName = property.type;
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
                        totalLines++;
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
