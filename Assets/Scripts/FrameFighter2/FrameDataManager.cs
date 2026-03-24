using FrameFighter2.Data;
using FrameFighter2.Hitbox;
using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;
using static FrameFighter2.Data.CharacterAnimationData;

namespace FrameFighter2.Manager
{
    [RequireComponent(typeof(Animator))]
    public class FrameDataManager : MonoBehaviour
    {

        [System.Serializable]
        public class CharacterAnimationEvent
        {
            [SerializeField] private string m_eventID;
            [SerializeField] private UnityEvent[] m_event; //made this an array to facilitate if multiple types of events (e.g. onEnter/onExit) | Mainly for active events

            public string EventID => m_eventID;
            public UnityEvent[] Event => m_event;

            public CharacterAnimationEvent(string eventID, UnityEvent[] unityEvent)
            {
                m_eventID = eventID;
                m_event = unityEvent;
            }

            public void Rename(string name)
            {
                m_eventID = name;
            }

            public void Reset(int i = 1)
            {
                m_event = new UnityEvent[i];
            }
        }

        [HideInInspector][SerializeField] private List<CharacterAnimationEvent> m_animationEvents = new(0);
        public List<CharacterAnimationEvent> AnimEvents => m_animationEvents;

        private Dictionary<string, UnityEvent[]> m_eventLookup;

        

        public CharacterAnimationEvent FindOrCreateEvent(string lookUp, int eventCount = 1)
        {
            
            foreach(CharacterAnimationEvent animEvent in m_animationEvents)
            {
                if (animEvent.EventID == lookUp) 
                {
                    if (animEvent.Event.Length != eventCount) animEvent.Reset(eventCount);
                    
                    return animEvent;
                }
            }

            CharacterAnimationEvent newEvent = new(lookUp, new UnityEvent[eventCount]);
            m_animationEvents.Add(newEvent);


            return newEvent;
        }

        public CharacterAnimationEvent FindEvent(string lookUp)
        {

            foreach (CharacterAnimationEvent animEvent in m_animationEvents)
            {
                if (animEvent.EventID == lookUp) return animEvent;
            }

            return null;
        }

        [HideInInspector][SerializeField] private List<CharacterAnimationData> m_characterAnimData = new(0); //set this to hide in inspector later

        public List<CharacterAnimationData> AnimData => m_characterAnimData;
        public void SetAnimData(List<CharacterAnimationData> newData)
        {
            m_characterAnimData = newData;
        }

        private Animator m_anim;
        private int m_currentStateHash;
        private AnimationClip m_currentClip;
        private CharacterAnimationData m_currentData;
        private AnimationClip[] m_clipList;
        private int m_lastFrame;

        private float m_progress;
        private int m_frame;
        private int m_lastLoopCount;

        private Dictionary<int, List<Collider>> m_hitObjects = new(); //keeps a list of objects that have been triggered 
        private List<HitboxObject> m_activeHitboxes = new();
        private List<EventData> m_activeEvents = new();

        private List<ComboInput> m_activeComboListers = new();

        private void Awake()
        {

            m_anim = GetComponent<Animator>();
            m_eventLookup = new Dictionary<string, UnityEvent[]>();

            foreach (var e in m_animationEvents)
            {
                m_eventLookup[e.EventID] = e.Event;
            }
            //get list of clips attatched to animator for comparison
            m_clipList = (m_anim.runtimeAnimatorController is AnimatorController animatorController) ? animatorController.animationClips : new AnimationClip[0];
        }

        private void Update()
        {
            //get current state
            AnimatorStateInfo state = m_anim.GetCurrentAnimatorStateInfo(0);

            //check if the animation has changed
            if(state.fullPathHash != m_currentStateHash)
            {
                //change current state hash
                m_currentStateHash = state.fullPathHash;
                //get currently playing clip
                m_currentClip = (m_anim.GetCurrentAnimatorClipInfo(0).Length != 0) ? m_anim.GetCurrentAnimatorClipInfo(0)[0].clip : null;
                //loop and find the data for the currently playing animation
                for (int i = 0; i < m_clipList.Length; i++)
                {
                    if (m_currentClip == m_clipList[i])
                    {
                        m_currentData = m_characterAnimData[i];
                        break;
                    }
                }

                //reset the currently hit objects
                m_hitObjects = new();
                //reset listeners
                ActiveEventCancel();
                ComboListenerCancel();
                //reset hitboxes
                DestroyAllHitboxes();
                //reset other variables
                m_lastLoopCount = 0;
                m_lastFrame = -1;
            }

            if (m_currentData == null || m_currentClip == null) return;

            //check if animation has looped to reset objects hit
            int loopCount = Mathf.FloorToInt(state.normalizedTime);
            if(loopCount > m_lastLoopCount)
            {
                m_hitObjects = new();
                m_lastLoopCount = loopCount;
            }

            // Calculate frame
            m_progress = state.normalizedTime % 1f;
            m_frame = Mathf.FloorToInt(m_progress * m_currentClip.frameRate * m_currentClip.length);

            if (m_frame != m_lastFrame)
            {
                m_lastFrame = m_frame;

                CheckFrameEvents(m_frame);
            }

        }
        /// <summary>
        /// goes through all data attached and invokes corresponding data
        /// </summary>
        private void CheckFrameEvents(int frame)
        {

            //check hitbox data and create them
            foreach(HitboxData hitboxData in m_currentData.HitboxData)
            {
                if(hitboxData.StartFrame == frame)
                {
                    CreateHitBox(hitboxData);
                    
                }
            }

            //check events
            foreach (EventData eventData in m_currentData.EventData)
            {
                if (eventData.StartFrame == frame)
                {
                    switch (eventData.EventType)
                    {
                        case EventData.EventTypes.Trigger:

                            //invoke trigger
                            InvokeEvent(eventData.EventID);

                            break;
                        case EventData.EventTypes.Active:

                            //invoke start event and set up end event
                            InvokeEvent(eventData.EventID, 0);
                            m_activeEvents.Add(eventData);

                            break;
                    }
                   
                }
            }
            
            //check active events
            for (int i = 0; i < m_activeEvents.Count; i++) 
            {
                EventData eventData = m_activeEvents[i];

                if (eventData.EndFrame == frame)
                {
                    //invoke event end
                    InvokeEvent(eventData.EventID, 1);
                    m_activeEvents.Remove(eventData);
                    i--;
                    
                }
            }

            //check active hitboxes and destroy old ones
            for (int i = 0; i < m_activeHitboxes.Count; i++)
            {
                HitboxObject hitbox = m_activeHitboxes[i];

                if (hitbox.EndFrame == frame)
                {
                    m_activeHitboxes.Remove(hitbox);
                    Destroy(hitbox.gameObject);
                    i--;
                }
            }

            //check combo input event (halen)
            if (m_currentData.NextComboInput.NextComboAttack != "" && m_currentData.NextComboInput.ComboInputTimeStart == frame)
            {
                Debug.Log("start combo input checking");
                m_activeComboListers.Add(m_currentData.NextComboInput);
            }

            for (int i = 0; i < m_activeComboListers.Count; i++)
            {
                ComboInput input = m_activeComboListers[i];

                if (input.ComboInputTimeEnd == frame)
                {
                    Debug.Log("End Combo Input checking");

                    m_activeComboListers.Remove(input);
                    i--;
                }

            }

        }

        /// <summary>
        /// creates a hitbox using the attached data
        /// </summary>
        public void CreateHitBox(HitboxData data)
        {
            GameObject hitbox = new GameObject(gameObject.name + "Hitbox");
            HitboxObject hitboxObject = hitbox.AddComponent<HitboxObject>();

            switch (data.HitboxType)
            {
                case HitboxData.HitboxTypes.Local:
                    hitbox.transform.parent = transform;
                    hitbox.transform.localPosition = data.Position;
                    break;
                case HitboxData.HitboxTypes.World:
                    hitbox.transform.position = Vector3.Scale((transform.position + data.Position), transform.lossyScale);

                    break;
                case HitboxData.HitboxTypes.AttachedToBone:
                    hitbox.transform.parent = FindObjectTransformFromPath(data.HitboxParent);
                    hitbox.transform.localPosition = data.Position;
                    break;
            }

            hitboxObject.Initialize(this, data.GroupID, data.OnHitEvent.EventID, (int)data.EndFrame, data.HitboxShape, data.Scale, data.Rotation, data.OnHitEffect);
            m_activeHitboxes.Add(hitboxObject);
        }
        /// <summary>
        /// Calls the end event of any currently active events. Call this function if an animation is canceled
        /// </summary>
        private void ActiveEventCancel()
        {
            for(int i = 0; i < m_activeEvents.Count; i++)
            {
                EventData activeEvent = m_activeEvents[i];

                //calls end event only if ShouldEndOnCancel is true
                if (activeEvent.ShouldEndOnCancel)
                {
                    InvokeEvent(activeEvent.EventID, 1);
                }
            }

            m_activeEvents = new();
        }

        private void ComboListenerCancel()
        {
            m_activeComboListers = new();
        }

        public void DestroyAllHitboxes()
        {
            //check active hitboxes and destroy old ones
            for (int i = 0; i < m_activeHitboxes.Count; i++)
            {
                HitboxObject hitbox = m_activeHitboxes[i];

                m_activeHitboxes.Remove(hitbox);
                Destroy(hitbox.gameObject);
                i--;

            }

            m_activeHitboxes = new();
        }

        /// <summary>
        /// evokes an event via the provided ID.
        /// </summary>
        public void InvokeEvent(string id, int eventIndex = 0)
        {
            if (m_eventLookup.TryGetValue(id, out var evt))
            {
                evt[eventIndex].Invoke();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if hit is allowed to be calculated</returns>
        public bool CheckHit(int groupID, Collider collider)
        {
            if (groupID == 0) return true;
            
            if (!m_hitObjects.ContainsKey(groupID))
            {
                m_hitObjects[groupID] = new()
                {
                    collider
                };
                return true;
            }

            for (int i = 0; i < m_hitObjects[groupID].Count; i++)
            {
                if (m_hitObjects[groupID][i] == collider)
                {
                    return false;
                }
            }

            m_hitObjects[groupID].Add(collider);
            return true;
        }

        private Transform FindObjectTransformFromPath(string pathString)
        {
            if (string.IsNullOrEmpty(pathString)) return null;

            int[] path = Array.ConvertAll(pathString.ToCharArray(), c => (int)Char.GetNumericValue(c));
            Transform current = transform;

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
    }
}


