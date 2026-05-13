using UnityEditor;
using UnityEngine;

namespace Stirge.Combat
{
    using Tools;
    
    [CustomPropertyDrawer(typeof(Status), true)]
    public class StatusDrawer : EasyPropertyDrawer
    {
        protected override void DrawGUI(GUIContent label)
        {
            if (m_property != null)
                label.text = m_property.managedReferenceValue.GetType().Name;
            else
                label.text = "Empty, pls remove";

            EditorGUI.BeginProperty(m_position, label, m_property);
            DrawLabelHeader(label);
            m_property.isExpanded = true; // Fixes display bug in Frame Data Viewer
            if (m_property.isExpanded)
            {
                if (m_property.managedReferenceValue is TimedStatus)
                    DrawPropertyField("m_length");
                switch (label.text)
                {
                    case nameof(AirJuggle):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_stallLength");
                        break;
                    case nameof(Knockback):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_height");
                        break;
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

                if (m_property.managedReferenceValue is TimedStatus)
                    totalLines++;

                switch (label.text)
                {
                    case nameof(AirJuggle):
                    case nameof(Knockback):
                        totalLines += 2;
                        break;
                    case nameof(Stun):
                        totalLines++;
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
