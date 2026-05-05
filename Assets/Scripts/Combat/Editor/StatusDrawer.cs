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
            string typeName = m_property.managedReferenceFullTypename;
            if (typeName.Length < 30)
                label.text = "Empty, pls delete";
            else
                label.text = typeName[30..];

            EditorGUI.BeginProperty(m_position, label, m_property);
            m_property.isExpanded = EditorGUI.Foldout(GetNewRect(), m_property.isExpanded, label);
            m_property.isExpanded = true;
            if (m_property.isExpanded)
            {
                switch (label.text)
                {
                    case nameof(AirJuggle):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_airStallLength");
                        DrawPropertyField("m_stunLength");
                        break;
                    case nameof(Knockback):
                        DrawPropertyField("m_strength");
                        DrawPropertyField("m_height");
                        DrawPropertyField("m_stunLength");
                        break;
                    case nameof(Stun):
                        DrawPropertyField("m_stunLength");
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
                string typeName = m_property.managedReferenceFullTypename;
                if (typeName.Length < 30)
                    typeName = string.Empty;
                else
                    typeName = typeName[30..];

                switch (typeName)
                {
                    case nameof(AirJuggle):
                    case nameof(Knockback):
                        totalLines += 3;
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
