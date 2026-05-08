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
            // get the string TypeName of the Condition
            string typeName = m_property.managedReferenceFullTypename;

            if (typeName.Length < 26)
            {
                label.text = "Empty, pls delete";
            }
            else
            {
                typeName = typeName[26..];
                label.text = typeName[..^9];

                // add "Not" to the start of a label if the value is inverted
                if (FindPropertyRelative("m_invertValue").boolValue)
                {
                    label.text = "Not " + label.text;
                }
            }
            
            EditorGUI.BeginProperty(m_position, label, m_property);
            m_property.isExpanded = EditorGUI.Foldout(GetNewRect(), m_property.isExpanded, label);
            if (m_property.isExpanded && label.text != "Empty, pls delete")
            {
                // draw the Invert Value prop
                DrawPropertyField("m_invertValue");

                if (typeName == nameof(DistanceCondition))
                    DrawPropertyField("m_distance");
            }

            EditorGUI.EndProperty();
        }

        protected override float GetHeight(GUIContent label)
        {
            int totalLines = 1; // for foldout

            string typeName = m_property.managedReferenceFullTypename;
            if (typeName.Length < 26)
                typeName = "";
            else
                typeName = typeName[26..];
            
            if (m_property.isExpanded && typeName != "")
            {
                totalLines++; // for invert value prop

                switch (typeName)
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
