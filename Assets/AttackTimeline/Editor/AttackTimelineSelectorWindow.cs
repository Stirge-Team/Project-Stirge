using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AttackTimelineSelectorWindow : EditorWindow
{
    [MenuItem("Tools/Timeline Selector")]
    public static void ShowWindow()
    {
        GetWindow<AttackTimelineSelectorWindow>("Timeline Selector");
    }

    string folderPath = "";
    PlayableDirector director = null;

    public void OnSelectionChange()
    {
        if (Application.isPlaying) return;
        
        //if a game object has been selected
        GameObject newTarget = Selection.activeGameObject;

        if (newTarget != null)
        {
            director = newTarget.GetComponent<PlayableDirector>();
        }
        
    }

    public void OnGUI()
    {
        if (director == null) { EditorGUILayout.HelpBox("Please Select a GameObject with a Director Attached", MessageType.Warning); }
        else { EditorGUILayout.HelpBox("Currently Selected Director: " + director.name, MessageType.Info); }

        EditorGUILayout.Space();

        folderPath = EditorGUILayout.DelayedTextField("Folder Path", folderPath);

        EditorGUILayout.Space();

        if (string.IsNullOrEmpty(folderPath))
        {
            return;
        }

        GUID[] timelineAssetGUID = AssetDatabase.FindAssetGUIDs("t:TimelineAsset", new[] { folderPath });
        TimelineAsset[] assets = new TimelineAsset[timelineAssetGUID.Length];

        for (int i = 0; i < timelineAssetGUID.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetByGUID<TimelineAsset>(timelineAssetGUID[i]);

        }

        if (assets != null) 
        {
            EditorGUI.BeginDisabledGroup(director == null);

            bool hasSelected = false;

            for (int i = 0; i < assets.Length; i++)
            {
                bool isSame = false;
                
                if (director != null) isSame = director.playableAsset == assets[i];

                EditorGUILayout.BeginHorizontal();

                string label = (isSame) ? assets[i].name + " (*)" : assets[i].name;

                EditorGUILayout.LabelField(label);

                EditorGUI.BeginDisabledGroup(isSame);

                if (GUILayout.Button("Set Asset"))
                {
                    director.playableAsset = assets[i];

                    EditorUtility.SetDirty(director.gameObject);
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();

                if (!hasSelected) hasSelected = isSame;
            }

            if (!hasSelected && director != null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(director.playableAsset.name);

                EditorGUILayout.LabelField("Currently Selected");

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
