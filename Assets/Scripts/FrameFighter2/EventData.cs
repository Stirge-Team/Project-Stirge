using UnityEngine;

namespace FrameFighter2.Data
{
    [System.Serializable]
    public class EventData
    {
        public enum EventTypes
        {
            Trigger,
            Active,
        }

        [SerializeField] private EventTypes m_eventType;
        [SerializeField] private string m_eventID;
        [SerializeField] private bool m_shouldEndOnCancel; //if the ActiveStop event should be called if an active event is cancelled

        [SerializeField] private float m_startFrame;
        [SerializeField] private float m_endFrame;

        public EventTypes EventType => m_eventType;
        public string EventID => m_eventID;
        public bool ShouldEndOnCancel => m_shouldEndOnCancel;
        public float StartFrame => m_startFrame;
        public float EndFrame => m_endFrame;

        public EventData (EventTypes eventType = EventTypes.Trigger, string eventID = "", bool resetOnCancel = false, float startFrame = 0, float endFrame = 0)
        {
            m_eventType = eventType;
            m_eventID = eventID;
            m_shouldEndOnCancel = resetOnCancel;
            m_startFrame = startFrame;
            m_endFrame = endFrame;
        }
        //as scriptable objects automatically construct any null values saved to them instead I'll check if the ID is null or empty
        /// <summary>
        /// Checks if the EventData is actually initialized
        /// </summary>
        /// <returns>true if an EventID has been assigned</returns>
        public bool DoesExist()
        {
            return !string.IsNullOrEmpty(m_eventID);
        }
        /// <summary>
        /// renames the eventID
        /// </summary>
        public void Rename(string name)
        {
            m_eventID = name;
        }
    }
}


