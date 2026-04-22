using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Animations;
using System;
using System.Collections.Generic;

using Stirge.Input;
using Stirge.Combat;

namespace FrameFighter2.Viewer
{
    using Manager;
    using Data;
    using static Data.HitboxData;
    using static Data.EventData;
    using static Manager.FrameDataManager;
    using static Data.CharacterAnimationData; 
    
    public class FrameDataViewer : EditorWindow
    {
        private GameObject m_gameObject;
        private Animator m_animator;
        private FrameDataManager m_manager;
        private AnimationClip[] m_clips;

        public bool HasClips()
        {
            return !(m_clips == null || m_clips.Length == 0);
        }

        private int m_selectedToolbarIndex;
        private string[] m_toolbarOptions = { "Hitbox Editor", "Data Viewer", "Options" };
        private int m_selectedAnimIndex;
        private float m_animPlayback;

        //options
        private int m_showPreviews = 0; //0 = show all, 1 = show timeline only, show none
        private string[] m_showPreviewOptions = { "Show All", "Show Only Active", "Hide" };

        //Used for foldouts in the hitbox creation window
        private Dictionary<string, bool> m_foldouts = new();
        private Vector2 m_globalScroll; //used for scrolling entire window

        bool DrawFoldout(string key, string label)
        {
            if (!m_foldouts.ContainsKey(key))
                m_foldouts[key] = true; // default state

            m_foldouts[key] = EditorGUILayout.BeginFoldoutHeaderGroup(m_foldouts[key], label);
            return m_foldouts[key];
        }

        private bool m_lockSelection = false;

        //Functions for creating and using a temp animation preview object
        private static GameObject m_previewAnimationCloneObject;
        private GameObject PreviewAnimationClone()
        {
            if (m_previewAnimationCloneObject == null)
            {
                m_previewAnimationCloneObject = Instantiate(m_gameObject);
                m_previewAnimationCloneObject.transform.position = m_gameObject.transform.position;
                m_previewAnimationCloneObject.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_previewAnimationCloneObject;
        }
        private static void DestroyPereviewAnimClone()
        {
            DestroyImmediate(m_previewAnimationCloneObject);
            m_previewAnimationCloneObject = null;
        }

        [MenuItem("Tools/Frame Data Viewer")]
        public static void ShowWindow()
        {
            GetWindow<FrameDataViewer>("Frame Data Viewer");
        }

        public void OnDestroy()
        {
            if (m_animator && HasClips())
            {
                ResetAnimaton(m_clips[0]);
            }

            Clear();
        }

        public void OnGUI()
        {
            GUILayout.Label("Frame Data Viewer", EditorStyles.boldLabel);

            if(Application.isPlaying) 
            {
                EditorGUILayout.HelpBox("No Editing in play mode", MessageType.Info);
                return; 
            }

            EditorGUILayout.BeginHorizontal();
            m_selectedToolbarIndex = GUILayout.Toolbar(m_selectedToolbarIndex, m_toolbarOptions, GUILayout.ExpandWidth(false));
            m_lockSelection = EditorGUILayout.Toggle(m_lockSelection, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            m_globalScroll = EditorGUILayout.BeginScrollView(m_globalScroll);

            //what toolbar item was selected
            switch (m_selectedToolbarIndex)
            {
                case 0: //Hitbox editor

                    //check if the animator is initialized
                    if (m_animator)
                    {
                        // Load Animation Clips
                        if (!HasClips())
                        {
                            m_clips = GetAnimatorClips(m_animator);
                            if (m_clips.Length == 0)
                            {
                                EditorGUILayout.HelpBox("No animation clips found in the Animator.", MessageType.Warning);
                                return;
                            }
                        }

                        EditorGUILayout.Space(10);

                        EditorGUILayout.LabelField("Animation Viewer", EditorStyles.boldLabel);

                        //check if the dropdown has been changed
                        EditorGUI.BeginChangeCheck();

                        // Animation Clip Dropdown
                        string[] clipNames = GetClipNames(m_clips);
                        m_selectedAnimIndex = EditorGUILayout.Popup("Animation Clip", m_selectedAnimIndex, clipNames, new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.MinWidth(250f) });
                        AnimationClip currentClip = m_clips[m_selectedAnimIndex];

                        if (EditorGUI.EndChangeCheck())
                        {
                            //if the selected clip index is currently larger than the animation data count, create a new blank slot (stops errors if the animator is updated while the window is open)
                            while (m_manager && m_selectedAnimIndex > m_manager.AnimData.Count - 1)
                            {
                                m_manager.AnimData.Add(null);
                            }

                            //sets timeline to 0 and updates preview
                            ResetAnimaton(currentClip);
                            CreatePreviews();
                        }

                        //check if the slider has been changed
                        EditorGUI.BeginChangeCheck();

                        m_animPlayback = EditorGUILayout.Slider("Animation Frame", m_animPlayback, 0, FrameCount(currentClip));

                        if (EditorGUI.EndChangeCheck())
                        {
                            m_animPlayback = Mathf.Round(m_animPlayback);
                            StepAnimation(currentClip);
                            CreatePreviews();
                        }
                        //Hitbox Creation UI
                        if (m_manager)
                        {
                            CharacterAnimationData data = (m_manager.AnimData.Count > 0) ? m_manager.AnimData[m_selectedAnimIndex] : null;

                            if (data)
                            {
                                EditorGUILayout.Space(20);

                                EditorGUILayout.LabelField("Hitboxes:", EditorStyles.boldLabel);

                                //for each hitbox attached to the data
                                for (int i = 0; i < data.HitboxData.Count; i++)
                                {
                                    EditorGUI.indentLevel = 0;

                                    EditorGUILayout.BeginHorizontal();

                                    //create foldout for easier navigation
                                    if (DrawFoldout(data.name + " Hitbox " + i, "Hitbox " + i + ":"))
                                    {
                                        //remove hitbox button
                                        if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                                        {
                                            Undo.RecordObject(data, "Removed hitbox data in " + data.name);

                                            //data.HitboxData[i] = null;
                                            //data.HitboxData.Remove(data.HitboxData[i]);

                                            RemoveHitbox(i);

                                            AnimationEventCleanup();

                                            CreatePreviews();

                                            EditorGUILayout.EndHorizontal();

                                            break;
                                        }

                                        EditorGUILayout.EndHorizontal();
                                        //create temp data
                                        HitboxData dataTemp = data.HitboxData[i];
                                        Vector3 position = dataTemp.Position;
                                        Vector3 rotation = dataTemp.Rotation;
                                        Vector3 scale = dataTemp.Scale;
                                        float frameStart = dataTemp.StartFrame;
                                        float frameEnd = dataTemp.EndFrame;
                                        int groupID = dataTemp.GroupID;
                                        var hitboxShape = dataTemp.HitboxShape;
                                        var hitboxType = dataTemp.HitboxType;
                                        string hitboxParent = dataTemp.HitboxParent;
                                        EventData onTrigger = dataTemp.OnHitEvent;
                                        SerializedObject serializedObject = new(m_manager);
                                        OnHitEffect onHitEffect = dataTemp.OnHitEffect;

                                        //Hitbox editing
                                        EditorGUI.BeginChangeCheck();
                                        //position, rotation, and scale
                                        position = EditorGUILayout.Vector3Field("Position: ", position);
                                        rotation = (EditorGUILayout.Vector3Field("Rotation: ", rotation));

                                        switch (hitboxShape)
                                        {
                                            case HitboxShapes.Rectangle:
                                                scale = EditorGUILayout.Vector3Field("Scale: ", scale);
                                                break;
                                            case HitboxShapes.Capsule:
                                                EditorGUILayout.BeginHorizontal();
                                                scale.x = EditorGUILayout.FloatField("Length: ", scale.x);
                                                scale.y = EditorGUILayout.FloatField("Radius: ", scale.y);
                                                EditorGUILayout.EndHorizontal();
                                                break;
                                            case HitboxShapes.Sphere:
                                                scale.x = EditorGUILayout.FloatField("Radius: ", scale.x);
                                                break;
                                        }

                                        EditorGUILayout.Space(10);

                                        //hitbox shape
                                        hitboxShape = (HitboxShapes)EditorGUILayout.EnumPopup("Hitbox Shape:", hitboxShape);
                                        //hitbox type
                                        hitboxType = (HitboxTypes)EditorGUILayout.EnumPopup("Hitbox Type:", hitboxType);

                                        if(hitboxType == HitboxTypes.AttachedToBone)
                                        {
                                            EditorGUILayout.Space(10);

                                            string parent = FindObjectNameFromPath(hitboxParent);

                                            EditorGUILayout.LabelField("Currently Selected Bone: " + parent);

                                            DrawTransformRecursive(m_gameObject.transform, 0, ref hitboxParent, i.ToString());

                                            EditorGUI.indentLevel = 0;

                                            EditorGUILayout.Space(10);
                                        }

                                        EditorGUILayout.Space(10);
                                        groupID = EditorGUILayout.IntField(new GUIContent("Group ID", "If a hitbox has been triggered on a specific object, " +
                                            "subsequent hitboxes with the same Group ID will not trigger when colliding on the same object. " +
                                            "Group ID of 0 will be considered ungrouped."), groupID);

                                        string groupedWith = "Group ID 0 assigns no group";

                                        if (groupID != 0)
                                        {
                                            string groupedHitboxes = "(";

                                            for (int j = 0; j < data.HitboxData.Count; j++)
                                            {
                                                if (i == j) continue;

                                                if (data.HitboxData[j].GroupID == groupID)
                                                {
                                                    groupedHitboxes += "Hitbox " + j + ", ";
                                                }
                                            }

                                            groupedHitboxes += ")";
                                            groupedWith = "Grouped With: " + groupedHitboxes;
                                        }

                                        EditorGUILayout.LabelField(groupedWith);

                                        EditorGUILayout.Space(10);

                                        //start frame and end frame
                                        EditorGUILayout.BeginHorizontal();
                                        frameStart = EditorGUILayout.IntField("Start Frame:", (int)frameStart);
                                        frameEnd = EditorGUILayout.IntField("End Frame:", (int)frameEnd);
                                        EditorGUILayout.EndHorizontal();
                                        
                                        //OnHit Events
                                        EditorGUILayout.Space(10);

                                        EditorGUILayout.MinMaxSlider(ref frameStart, ref frameEnd, 0, FrameCount(currentClip));

                                        if (onTrigger != null)
                                        {
                                            if (onTrigger.DoesExist())
                                            {
                                                EditorGUILayout.BeginHorizontal();

                                                EditorGUILayout.LabelField("On Hit Event:");

                                                if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                                                {
                                                    onTrigger = new(EventTypes.Trigger, "");
                                                }
                                                
                                                EditorGUILayout.EndHorizontal();
                                                
                                                UnityEvent currentEvent = m_manager.FindOrCreateEvent(data.name + "OnHit" + i).Event[0]; //as on hit events will always only need one event the index will always be 0

                                                if (currentEvent != null)
                                                {
                                                    SerializedProperty eventSerialized = (FindAnimationEventIndex(data.name + "OnHit" + i) + 1 > serializedObject.FindProperty("m_animationEvents").arraySize) ? null : 
                                                        serializedObject.FindProperty("m_animationEvents").GetArrayElementAtIndex(FindAnimationEventIndex(data.name + "OnHit" + i)).FindPropertyRelative("m_event").GetArrayElementAtIndex(0);

                                                    if (eventSerialized != null)
                                                    {
                                                        EditorGUILayout.PropertyField(eventSerialized);
                                                    }
                                                }
                                            }
                                            else 
                                            { 
                                                if (GUILayout.Button("Add On Hit Event"))
                                                {
                                                    onTrigger = new(EventTypes.Trigger, data.name + "OnHit" + i);
                                                }

                                            }
                                        }

                                        EditorGUILayout.Space(10);

                                        //OnHitEffect


                                        //SerializedProperty onHitEffectSerialized = serializedObject.FindProperty("m_characterAnimData").GetArrayElementAtIndex(m_selectedAnimIndex);

                                        SerializedObject scriptableOjbect = new(m_manager.AnimData[m_selectedAnimIndex]);
                                        SerializedProperty onHitEffectSerialized = scriptableOjbect.FindProperty("m_hitboxData").GetArrayElementAtIndex(i).FindPropertyRelative("m_onHitEffect");

                                        EditorGUILayout.PropertyField(onHitEffectSerialized);

                                        //FindPropertyRelative("m_hitboxData").GetArrayElementAtIndex(i);

                                        //apply changes
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(data, "Changed data in " + data.name);

                                            //clamp and round results
                                            frameStart = Mathf.Clamp(frameStart, 0, FrameCount(currentClip));
                                            frameEnd = Mathf.Clamp(frameEnd, frameStart, FrameCount(currentClip));
                                            frameStart = Mathf.Round(frameStart);
                                            frameEnd = Mathf.Round(frameEnd);

                                            scale = new(Mathf.Clamp(scale.x, 0, Mathf.Infinity), Mathf.Clamp(scale.y, 0, Mathf.Infinity), Mathf.Clamp(scale.z, 0, Mathf.Infinity));

                                            data.EditHitBox(i, new(position, rotation, scale, frameStart, frameEnd, groupID, hitboxShape, hitboxType, hitboxParent, onTrigger, onHitEffect));

                                            serializedObject.ApplyModifiedProperties();
                                            scriptableOjbect.ApplyModifiedProperties();

                                            AnimationEventCleanup();

                                            EditorUtility.SetDirty(data);
                                            
                                            CreatePreviews();

                                        }
                                        EditorGUILayout.Space(20);
                                    }
                                    else EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndFoldoutHeaderGroup();

                                }
                                EditorGUI.indentLevel = 0;

                                //create hitbox button
                                if (GUILayout.Button("Create Hitbox"))
                                {
                                    //HitboxPreview.CreatePreview(m_gameObject.transform.position, Quaternion.identity, Vector3.one);
                                    Undo.RecordObject(data, "Created new hitbox data in " + data.name);
                                    data.CreateHitbox();

                                    CreatePreviews();
                                }

                                EditorGUILayout.HelpBox("Total hitboxes in animation: " + data.HitboxData.Count, MessageType.None);

                                EditorGUILayout.Space(20);

                                EditorGUILayout.LabelField("Events:", EditorStyles.boldLabel);

                                //for each event attatched to the data
                                for (int i = 0; i < data.EventData.Count; i++)
                                {
                                    EditorGUILayout.BeginHorizontal();

                                    //create foldout for easier navigation
                                    if (DrawFoldout(data.name + " Event " + i, "Event " + i + ":"))
                                    {
                                        
                                        //remove hitbox button
                                        if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                                        {
                                            Undo.RecordObject(data, "Removed event data in " + data.name);

                                            EditorGUILayout.EndHorizontal();

                                            AnimationEventCleanup();

                                            RemoveEvent(i);

                                            break;
                                        }

                                        EditorGUILayout.EndHorizontal();

                                        //create temp data
                                        EventData eventData = data.EventData[i];

                                        string eventID = eventData.EventID;
                                        float frameStart = eventData.StartFrame;
                                        float frameEnd = eventData.EndFrame;
                                        EventTypes eventType = eventData.EventType;
                                        bool shouldEndOnCancel = eventData.ShouldEndOnCancel;

                                        SerializedObject serializedObject = new(m_manager);

                                        EditorGUI.BeginChangeCheck();

                                        eventType = (EventTypes)EditorGUILayout.EnumPopup("Event Type", eventType);
                                        int eventCount = 1;
                                        string[] titleText = new string[] {""}; //text that will appear over the unity event
                                        
                                        switch (eventType)
                                        {
                                            case EventTypes.Trigger:
                                                //frameStart = (float)EditorGUILayout.IntField("Trigger Frame", (int)frameStart);
                                                frameStart = EditorGUILayout.IntSlider("Trigger Frame", (int)frameStart, 0, FrameCount(currentClip));

                                                titleText[0] = "";

                                                titleText = new string[] { "Trigger Event" };

                                                break;
                                            case EventTypes.Active:

                                                EditorGUILayout.BeginHorizontal();
                                                frameStart = EditorGUILayout.IntField("Start Frame", (int)frameStart);
                                                frameEnd = EditorGUILayout.IntField("End Frame", (int)frameEnd);
                                                EditorGUILayout.EndHorizontal();

                                                EditorGUILayout.MinMaxSlider(ref frameStart, ref frameEnd, 0, FrameCount(currentClip));

                                                shouldEndOnCancel = EditorGUILayout.Toggle(new GUIContent("End when Canceled", "If an animation is canceled, should the end event be called."), shouldEndOnCancel);

                                                eventCount = 2; // one event for when the event starts and one event for when it finishes
                                                titleText = new string[] { "On Event Start", "On Event Finish" };

                                                break;
                                        }

                                        UnityEvent[] currentEvents = m_manager.FindOrCreateEvent(data.name + "Event" + i, eventCount).Event;

                                        if (currentEvents != null)
                                        {
                                            bool eventOutsideRange = FindAnimationEventIndex(data.name + "Event" + i) + 1 > serializedObject.FindProperty("m_animationEvents").arraySize;

                                            for (int j = 0; j < currentEvents.Length; j++)
                                            {
                                                EditorGUILayout.LabelField(titleText[j]);

                                                bool unityEventOutsideRange = (eventOutsideRange) ? true : j + 1 > serializedObject.FindProperty("m_animationEvents").GetArrayElementAtIndex(FindAnimationEventIndex(data.name + "Event" + i))
                                                    .FindPropertyRelative("m_event").arraySize;

                                                SerializedProperty eventSerialized = (eventOutsideRange || unityEventOutsideRange) ? null : 
                                                    serializedObject.FindProperty("m_animationEvents").GetArrayElementAtIndex(FindAnimationEventIndex(data.name + "Event" + i))
                                                    .FindPropertyRelative("m_event").GetArrayElementAtIndex(j);

                                                if (eventSerialized != null)
                                                {
                                                    EditorGUILayout.PropertyField(eventSerialized);
                                                }
                                            }
                                        }

                                        //UnityEvent currentEvent = m_manager.FindOrCreateEvent(data.name + "Event" + i).Event;


                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            //clamp results
                                            frameStart = Mathf.Clamp(Mathf.Round(frameStart), 0, FrameCount(currentClip));
                                            frameEnd = Mathf.Clamp(Mathf.Round(frameEnd), frameStart, FrameCount(currentClip));

                                            Undo.RecordObject(data, "Changed data in " + data.name);

                                            data.EditEvent(i, new(eventType, eventID, shouldEndOnCancel, frameStart, frameEnd));

                                            serializedObject.ApplyModifiedProperties();

                                            AnimationEventCleanup();

                                            EditorUtility.SetDirty(data);
                                        }

                                        EditorGUILayout.Space(20);

                                    }
                                    else EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.EndFoldoutHeaderGroup();


                                }
                                
                                //create event button
                                if (GUILayout.Button("Create Event"))
                                {
                                    Undo.RecordObject(data, "Created new event data in " + data.name);
                                    data.CreateEvent(data.name + "Event" + data.EventData.Count);

                                    Repaint();

                                    CreatePreviews();

                                }
                                EditorGUILayout.HelpBox("Total Events in animation: " + data.EventData.Count, MessageType.None);

                                //This stops a purely visual error from appearing (when you create a new event). I know this is jank as hell but it doesn't affect the main system's functionality.
                                //My theory is that it's because the property field function doesn't like showing unity events the update they are created?
                                //But I have also set up systems where it delays rendering them and it also did nothing. Also Why does the error occur at the create event button? I hate unity. 
                                try
                                {
                                    EditorGUILayout.Space(20);
                                }
                                catch{
                                    //Debug.Log("borked it");
                                }

                                //Halen's Combo Input System
                                EditorGUILayout.LabelField("Combo Input:", EditorStyles.boldLabel);

                                ComboInput comboInput = data.NextComboInput;

                                string nextComboAttack = comboInput.NextComboAttack;
                                AttackInput comboAttackInput = comboInput.ComboAttackInput;
                                float comboInputTimeStart = comboInput.ComboInputTimeStart;
                                float comboInputTimeEnd = comboInput.ComboInputTimeEnd;

                                EditorGUI.BeginChangeCheck();

                                nextComboAttack = EditorGUILayout.TextField("Next Attack In Combo", nextComboAttack);

                                if (nextComboAttack != "")
                                {
                                    comboAttackInput = (AttackInput)EditorGUILayout.EnumPopup("Next Input" ,comboAttackInput);

                                    EditorGUILayout.BeginHorizontal();
                                    comboInputTimeStart = EditorGUILayout.FloatField("Input Start", Mathf.Round(comboInputTimeStart));
                                    comboInputTimeEnd = EditorGUILayout.FloatField("Input End", Mathf.Round(comboInputTimeEnd));
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.MinMaxSlider(ref comboInputTimeStart, ref comboInputTimeEnd, 0, FrameCount(currentClip));

                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("Blank Field means the Animation does not have a combo", MessageType.None);
                                }

                                if (EditorGUI.EndChangeCheck())
                                {
                                    comboInputTimeStart = Mathf.Clamp(MathF.Round(comboInputTimeStart), 0, FrameCount(currentClip));
                                    comboInputTimeEnd = Mathf.Clamp(MathF.Round(comboInputTimeEnd), comboInputTimeStart, FrameCount(currentClip));

                                    Undo.RecordObject(data, "Created new event data in " + data.name);

                                    data.EditComboData(new(nextComboAttack, comboAttackInput, comboInputTimeStart, comboInputTimeEnd));

                                    EditorUtility.SetDirty(data);
                                }

                                EditorGUILayout.Space(20);

                            }
                            else
                            {
                                EditorGUILayout.HelpBox("No animation Character Animation Data attatched to animation.", MessageType.None);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No Frame Data Manager attatched to Object.", MessageType.Warning);
                        }


                        //duplicate animation name check
                        if (CheckForDuplicates(clipNames, out List<string> duplicates))
                        {
                            string formatted = "";
                            for (int i = 0; i < duplicates.Count; i++)
                            {
                                formatted += "\n" + duplicates[i];
                            }

                            EditorGUILayout.HelpBox("Animation clips with duplicate names detected! This may cause unintended issues. It is recommended to rename the duplicate animation clip even if they are the same. Found clips:" + formatted, MessageType.Warning);
                        }

                    }
                    else
                    {
                        if (Selection.activeGameObject)
                        {
                            EditorGUILayout.HelpBox("This GameObject does not have an Animator attatched.", MessageType.Warning);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Please select a GameObject.", MessageType.Info);
                        }

                    }

                    break;
                case 1: //Data viewer
                    
                    //if object has a manager attatched
                    if (m_manager)
                    {
                        // Load Animation Clips
                        if (!HasClips())
                        {
                            m_clips = GetAnimatorClips(m_animator);
                            if (m_clips.Length == 0)
                            {
                                EditorGUILayout.HelpBox("No animation clips found in the Animator.", MessageType.Warning);
                                return;
                            }
                        }

                        string[] clipNames = GetClipNames(m_clips);

                        EditorGUI.BeginChangeCheck();

                        List<CharacterAnimationData> dataTemp = new();
                        //fills out the temp list with dummy values for each list option (stops an error)
                        for (int i = 0; i < m_clips.Length; i++)
                        {
                            dataTemp.Add(null);
                        }

                        for (int i = 0; i < m_clips.Length; i++) 
                        {
                            EditorGUILayout.BeginHorizontal();

                            //gets current list item data for temp list, returns null if it exceeds the current list size
                            CharacterAnimationData data = (m_manager.AnimData.Count > i) ? m_manager.AnimData[i] : null;

                            dataTemp[i] = (CharacterAnimationData)EditorGUILayout.ObjectField(clipNames[i], data, typeof(CharacterAnimationData), false);

                            //Button for creating a new characteranimationdata object if slot is empty
                            if (!dataTemp[i])
                            {
                                if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
                                {
                                    CharacterAnimationData newScriptableObject = CreateInstance<CharacterAnimationData>();
                                    newScriptableObject.name = clipNames[i];

                                    AssetDatabase.CreateAsset(newScriptableObject, "Assets/" + newScriptableObject.name + ".asset");

                                    dataTemp[i] = newScriptableObject;

                                    newScriptableObject.ClearHitboxes();

                                    Debug.Log("Create new asset \"" + newScriptableObject.name + "\" in Assets folder.");
                                }
                            }
                            

                            EditorGUILayout.EndHorizontal();
                            
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(m_manager, "Changed data in " + m_manager.name);
                            m_manager.SetAnimData(dataTemp);
                        }
                    }
                    else
                    {
                        string msg = (Selection.activeGameObject) ? "No Frame Data Manager attatched to Object." : "Please select a GameObject.";

                        EditorGUILayout.HelpBox(msg, MessageType.Warning);
                    }
                    
                    break;
                case 2: //Options

                    //check if the dropdown has been changed
                    EditorGUI.BeginChangeCheck();

                    // Hitbox Previews Dropdown
                    m_showPreviews = EditorGUILayout.Popup("Show Previews", m_showPreviews, m_showPreviewOptions, new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.MinWidth(250f) });

                    if (EditorGUI.EndChangeCheck())
                    {
                        //updates preview
                        CreatePreviews();
                    }

                    break;
            }

            EditorGUILayout.EndScrollView();

        }

        public void OnSelectionChange()
        {
            if (m_lockSelection || Application.isPlaying) return;

            //if a game object has been selected
            if (Selection.activeGameObject)
            {
                //sets character back to default
                if (HasClips()) ResetAnimaton(m_clips[0]);

                //clears values
                Clear();

                //sets selected game object
                m_gameObject = Selection.activeGameObject;

                //sets animator
                m_animator = m_gameObject.GetComponent<Animator>();

                //sets frame data manager
                m_manager = m_gameObject.GetComponent<FrameDataManager>();

                if (m_animator != null)
                {
                    //hides selected object create new preview clone
                    SceneVisibilityManager.instance.Hide(m_gameObject, true);
                    PreviewAnimationClone();
                }

                if (m_manager != null)
                {
                    CharacterAnimationData data = (m_manager.AnimData.Count > 0) ? m_manager.AnimData[0] : null;
                    if (data) CreatePreviews();
                }

            }
            else if (m_animator) // if no game object has been selected (but animator is still referenced)
            {
                //sets character back to default
                if (HasClips()) ResetAnimaton(m_clips[0]);
                //clears values
                Clear();
            }

            Repaint();
        }

        private int FrameCount(AnimationClip clip)
        {
            return Mathf.RoundToInt(clip.length * clip.frameRate);
        }

        private float FramerateDelta(AnimationClip clip)
        {
            return 1f / clip.frameRate;
        }

        private void StepAnimation(AnimationClip clip)
        {
            if (clip == null || m_animator == null) return;

            // Apply to animator
            clip.SampleAnimation(PreviewAnimationClone(), m_animPlayback * FramerateDelta(clip));

            // Refresh Scene View
            SceneView.RepaintAll();
        }

        private AnimationClip[] GetAnimatorClips(Animator animator)
        {
            if (animator.runtimeAnimatorController is AnimatorController animatorController)
            {
                return animatorController.animationClips;
            }
            return new AnimationClip[0];
        }


        private string[] GetClipNames(AnimationClip[] clips)
        {
            string[] names = new string[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                names[i] = clips[i].name;
            }
            return names;
        }
        /// <summary>
        /// checks if any of the strings in the inputted array are duplicates
        /// </summary>
        /// <param name="names">list of strings to check</param>
        /// <returns>true if the array contains duplicates</returns>
        private bool CheckForDuplicates(string[] names, out List<string> duplicates)
        {
            HashSet<string> stringSet = new HashSet<string>();
            duplicates = new List<string>();

            bool found = false;

            foreach (string s in names)
            {
                if (!stringSet.Add(s))
                {
                    found = true;
                    if (!duplicates.Contains(s)) // Avoid adding the same duplicate multiple times
                    {
                        duplicates.Add(s);
                    }
                }
            }

            return found;
        }

        private string CreateObjectPath(Transform startObject)
        {
            bool found = (startObject == m_gameObject.transform);

            if (found) return "Error";

            string path = startObject.GetSiblingIndex().ToString();

            Transform current = startObject;

            while (!found)
            {
                current = current.parent;
                found = (current == m_gameObject.transform);

                if (!found) path = path.Insert(0, current.GetSiblingIndex().ToString());

                
            }

            return path;
        }

        private string FindObjectNameFromPath(string pathString)
        {
            if (string.IsNullOrEmpty(pathString)) return null;

            int[] path = Array.ConvertAll(pathString.ToCharArray(), c => (int)Char.GetNumericValue(c));
            string result = "null";
            Transform current = m_gameObject.transform;

            try
            {
                for (int i = 0; i < path.Length; i++)
                {
                    current = current.GetChild(path[i]);
                }
            }
            catch
            {
                result = "null";
            }

            result = current.name;

            return result;
        }

        private Transform FindObjectTransformFromPath(string pathString)
        {
            if (string.IsNullOrEmpty(pathString)) return null;

            int[] path = Array.ConvertAll(pathString.ToCharArray(), c => (int)Char.GetNumericValue(c));
            Transform current = m_gameObject.transform;

            try
            {
                for (int i = 0; i < path.Length; i++)
                {
                    current = current.GetChild(path[i]);
                }
            }
            catch
            {
                current = null;
            }

            return current;
        }

        /// <summary>
        /// searches all animation events attatched to the manager until it finds the result matching the ID
        /// </summary>
        /// <returns>Index of where the event is</returns>
        private int FindAnimationEventIndex(string eventID)
        {
            int i;
            for (i = 0; i < m_manager.AnimEvents.Count; i++)
            {
                if (m_manager.AnimEvents[i].EventID == eventID) return i;
            }

            return -1;
        }
        /// <summary>
        /// searches all event slots attatched to the manager until it finds the result matching the ID
        /// </summary>
        /// <returns>Index of where the event is</returns>
        private int FindEventSlotIndex(string eventID)
        {
            foreach(CharacterAnimationData data in m_manager.AnimData)
            {
                if (data == null) continue;

                string[] events = data.GetEvents();

                int i;
                for (i = 0; i < events.Length; i++)
                {
                    if (events[i] == eventID) return i;
                }
            }
            return -1;
        }

        private void AnimationEventCleanup()
        {
            //List<CharacterAnimationEvent> animEventsTemp = m_manager.AnimEvents;

            for (int i = 0; i < m_manager.AnimEvents.Count; i++)
            {
                if (FindEventSlotIndex(m_manager.AnimEvents[i].EventID) == -1)
                {
                    Debug.Log("Removing: " + m_manager.AnimEvents[i].EventID);
                    m_manager.AnimEvents.Remove(m_manager.AnimEvents[i]);
                }
            }
        }

        private void RemoveHitbox(int hitboxToRemove)
        {
            CharacterAnimationData data = m_manager.AnimData[m_selectedAnimIndex];

            m_manager.AnimEvents.Remove(m_manager.FindEvent(data.name + "OnHit" + hitboxToRemove));

            for (int i = hitboxToRemove + 1; i < data.HitboxData.Count; i++)
            {
                if (data.HitboxData[i].OnHitEvent.DoesExist())
                {
                    data.HitboxData[i].OnHitEvent.Rename(data.name + "OnHit" + (i - 1));

                    CharacterAnimationEvent currentEvent = m_manager.FindEvent(data.name + "OnHit" + i);

                    currentEvent.Rename(data.name + "OnHit" + (i - 1));
                }
            }

            data.HitboxData[hitboxToRemove] = null;
            data.HitboxData.Remove(data.HitboxData[hitboxToRemove]);

            EditorUtility.SetDirty(data);

        }

        private void RemoveEvent(int eventToRemove)
        {
            CharacterAnimationData data = m_manager.AnimData[m_selectedAnimIndex];

            m_manager.AnimEvents.Remove(m_manager.FindEvent(data.name + "Event" + eventToRemove));

            for (int i = eventToRemove + 1; i < data.EventData.Count; i++)
            {
                if (data.EventData[i].DoesExist())
                {
                    data.EventData[i].Rename(data.name + "Event" + (i - 1));

                    CharacterAnimationEvent currentEvent = m_manager.FindEvent(data.name + "Event" + i);

                    currentEvent.Rename(data.name + "Event" + (i - 1));
                }
            }

            data.EventData[eventToRemove] = null;
            data.EventData.Remove(data.EventData[eventToRemove]);

            EditorUtility.SetDirty(data);

        }

        /// <summary>
        /// clears all attatched object data
        /// </summary>
        private void Clear()
        {
            if (m_gameObject != null) SceneVisibilityManager.instance.Show(m_gameObject, true);
            m_gameObject = null;
            m_clips = null;
            m_animator = null;
            m_manager = null;
            m_selectedAnimIndex = 0;

            DestroyPereviewAnimClone();
        }

        private void ResetAnimaton(AnimationClip clip)
        {
            HitboxPreview.DestroyAllPreviews();

            //sets timeline back to start
            m_animPlayback = 0f;

            // Apply to animator
            StepAnimation(clip);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="indent"></param>
        /// <param name="pathToGet">String to edit with the new path if a button is pressed</param>
        /// <param name="key"></param>
        private void DrawTransformRecursive(Transform t, int indent, ref string pathToGet, string key = "0")
        {

            EditorGUI.indentLevel = indent;

            string id = t.gameObject.name + key;

            if (!m_foldouts.ContainsKey(id))
                m_foldouts[id] = false;

            bool hasChildren = t.childCount > 0;

            if (hasChildren)
            {
                EditorGUILayout.BeginHorizontal();
                m_foldouts[id] = EditorGUILayout.Foldout(
                    m_foldouts[id],
                    t.name,
                    true
                );

                if (GUILayout.Button("<", GUILayout.MaxWidth(20)))
                {
                    pathToGet = CreateObjectPath(t);
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(t.name);

                if (GUILayout.Button("<", GUILayout.MaxWidth(20)))
                {
                    pathToGet = CreateObjectPath(t);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (hasChildren && m_foldouts[id])
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    if (t.GetChild(i).gameObject.hideFlags == HideFlags.HideAndDontSave) continue;

                    DrawTransformRecursive(t.GetChild(i), indent + 1, ref pathToGet);
                }
            }
        }

        

        /// <summary>
        /// creates all applicable previews
        /// </summary>
        private void CreatePreviews()
        {
            //TODO: Different Hitbox shapes!!!!!
            HitboxPreview.DestroyAllPreviews();

            if (!m_manager || m_showPreviews == 2) return;
            CharacterAnimationData data = (m_manager.AnimData.Count > 0) ? m_manager.AnimData[m_selectedAnimIndex] : null;
            if (!data) return;

            for (int i = 0; i < data.HitboxData.Count; i++)
            {
                bool isActive = (m_animPlayback >= data.HitboxData[i].StartFrame && m_animPlayback <= data.HitboxData[i].EndFrame);

                //if show only active is enabled skip if not in applicable range
                if (!isActive && m_showPreviews == 1) continue;

                Vector3 position = data.HitboxData[i].Position;
                Vector3 scale = data.HitboxData[i].Scale;
                Vector3 rotation = data.HitboxData[i].Rotation;

                HitboxShapes shape = data.HitboxData[i].HitboxShape;

                HitboxTypes types = data.HitboxData[i].HitboxType;
                string path = data.HitboxData[i].HitboxParent;

                Transform parent = PreviewAnimationClone().transform;

                //finds bone to attatch to if applicable
                if (types == HitboxTypes.AttachedToBone)
                {
                    parent = FindObjectTransformFromPath(path);
                }
                //Adjusts scales to match up with shape (this is purely for visual parity)
                Vector3 finalScale = shape switch
                {
                    HitboxShapes.Rectangle => scale,
                    HitboxShapes.Capsule => new Vector3(scale.y, scale.x, scale.y),
                    HitboxShapes.Sphere => new Vector3(scale.x, scale.x, scale.x),
                    _ => throw new NotImplementedException()
                };

                HitboxPreview.CreatePreview(parent, position, rotation, finalScale, shape, isActive);
            }
        }

        [InitializeOnLoad]
        static class PreviewCleanup
        {
            static PreviewCleanup()
            {
                EditorApplication.playModeStateChanged += _ =>
                {
                    DestroyPereviewAnimClone();
                };

                AssemblyReloadEvents.beforeAssemblyReload += DestroyPereviewAnimClone;

            }
        }
    }

}