using System;
using System.Collections.Generic;
using Stirge.Environment;
using UnityEditor;
using UnityEngine;

namespace Stirge
{
    [CustomEditor(typeof(SimpleTriggerBox))]
    public class SimpleTriggerBoxEditor : Editor
    {
        private List<Transform> m_colliderPreviews;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SimpleTriggerBox triggerBox = (SimpleTriggerBox)target;
            if(GUILayout.Button("Render Previews"))
            {
                triggerBox.RenderPreviews();
            }
        }
    }
}
