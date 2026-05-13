using UnityEditor;
using UnityEngine;

namespace Stirge.AI
{
    using Tools;
    
    [CustomPropertyDrawer(typeof(Condition))]
    public class ConditionDrawer : EasyPropertyDrawer
    {
        protected override void DrawGUI(GUIContent label)
        {
            if (m_property != null)
                label.text = m_property.managedReferenceValue.GetType().Name;
            else
                label.text = "Empty, pls delete";

            // add "Not" to the start of a label if the value is inverted
            if (FindPropertyRelative("m_invertValue").boolValue)
            {
                label.text = "Not " + label.text;
            }
            
            EditorGUI.BeginProperty(m_position, label, m_property);
            DrawLabelHeader(label);
            if (m_property.isExpanded && label.text != "Empty, pls delete")
            {
                // draw the Invert Value prop
                DrawPropertyField("m_invertValue");

                if (label.text == nameof(DistanceCondition))
                    DrawPropertyField("m_distance");
            }

            EditorGUI.EndProperty();
        }

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1; // for foldout/label

            SetLabelTextToTypeName(label);

            if (m_property.isExpanded && !label.text.Contains(','))
            {
                totalLines++; // for invert value prop

                switch (label.text)
                {
                    case nameof(DistanceCondition):
                        totalLines++;
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
