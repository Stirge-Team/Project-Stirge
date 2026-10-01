using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using EG = UnityEditor.EditorGUI;
using EGL = UnityEditor.EditorGUILayout;

namespace Stirge.Tools
{
    public abstract class EasyCustomEditor : Editor
    {
        private bool m_hasChanged;

        private Dictionary<string, SerializedProperty> m_cachedProperties = new();

        protected void HasChanged()
        {
            m_hasChanged = true;
        }

        private void OnEnable()
        {
            m_cachedProperties.Clear();
            OnEnableThis();
        }

        protected abstract void OnEnableThis();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EG.BeginChangeCheck();

            // show Script fields for quick editing
            using (new EG.DisabledScope(true))
            {
                if (target is ScriptableObject)
                {
                    EGL.ObjectField("Script", MonoScript.FromScriptableObject((ScriptableObject)target), target.GetType(), false);
                    EGL.ObjectField("Editor", MonoScript.FromScriptableObject(this), GetType(), false);
                }
                else
                {
                    EGL.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), target.GetType(), false);
                    EGL.ObjectField("Editor", MonoScript.FromScriptableObject(this), GetType(), false);
                }
            }

            OnGUI();

            if (EG.EndChangeCheck() || m_hasChanged)
            {
                m_hasChanged = false;
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected abstract void OnGUI();
    }
}
