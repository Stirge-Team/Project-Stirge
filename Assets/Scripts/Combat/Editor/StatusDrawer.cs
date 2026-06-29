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

            string typeName = label.text;

            EditorGUI.BeginProperty(m_position, label, m_property);
            DrawLabelHeader(label);
            m_property.isExpanded = true; // Fixes display bug in Frame Data Viewer
            if (m_property.isExpanded)
            {
                if (m_property.managedReferenceValue is TimedStatus)
                    DrawPropertyField("m_length");

                switch (typeName)
                {
                    case nameof(AirJuggle):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_stallLength");
                        break;
                    case nameof(Knockback):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_height");
                        break;
                    case nameof(HitStopStatus):
                        DrawPropertyField("m_duration");
                        DrawPropertyField("m_scale");
                        break;
                    case nameof(ScreenShakeEffect):
                        DrawPropertyField("m_preset");
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

                string typeName = label.text;

                if (m_property.managedReferenceValue is TimedStatus)
                    totalLines++;

                switch (typeName)
                {
                    case nameof(AirJuggle):
                    case nameof(Knockback):
                    case nameof(HitStopStatus):
                        totalLines += 2;
                        break;
                    case nameof(Stun):
                    case nameof(ScreenShakeEffect):
                        totalLines++;
                        break;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
        }
    }
}
