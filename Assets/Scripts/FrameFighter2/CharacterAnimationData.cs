using Stirge.Input;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace FrameFighter2.Data
{
    //this is the stored data for each individual animation on the hitbox data

    [CreateAssetMenu(fileName = "CharacterAnimationData", menuName = "Scriptable Objects/CharacterAnimationData")]
    public class CharacterAnimationData : ScriptableObject
    {

        [SerializeField] private List<HitboxData> m_hitboxData = new(0);
        [SerializeField] private List<EventData> m_eventData = new(0);

        public List<HitboxData> HitboxData => m_hitboxData;
        public List<EventData> EventData => m_eventData;

        #region HalenInput
        [System.Serializable]
        public class ComboInput
        {
            
            //Combination inputs for halen's input system
            [SerializeField] private string m_nextComboAttack;
            [SerializeField] private AttackInput m_comboAttackInput;
            [SerializeField] private float m_comboInputTimeStart;
            [SerializeField] private float m_comboInputTimeEnd;

            public string NextComboAttack => m_nextComboAttack;
            public AttackInput ComboAttackInput => m_comboAttackInput;
            public float ComboInputTimeStart => m_comboInputTimeStart;
            public float ComboInputTimeEnd => m_comboInputTimeEnd;
            

            public ComboInput(string nextComboAttack, AttackInput comboAttackInput, float comboInputTimeStart, float comboInputTimeEnd)
            {
                m_nextComboAttack = nextComboAttack;
                m_comboAttackInput = comboAttackInput;
                m_comboInputTimeStart = comboInputTimeStart;
                m_comboInputTimeEnd = comboInputTimeEnd;
            }
        }
        #endregion

        [SerializeField] private ComboInput m_nextComboInput;
        public ComboInput NextComboInput => m_nextComboInput;

        public string[] GetEvents() {
            List<string> events = new List<string>();
            
            foreach (HitboxData hitboxData in m_hitboxData)
            {
                if(hitboxData.OnHitEvent.DoesExist()) events.Add(hitboxData.OnHitEvent.EventID);
            }
            foreach(EventData eventData in m_eventData)
            {
                events.Add(eventData.EventID);
            }

            return events.ToArray();
        }

        public void CreateHitbox(int i)
        {
            HitboxData newHitbox = new(Vector3.zero, Vector3.zero, Vector3.one, 0f, 0f);
            m_hitboxData.Insert(i, newHitbox);
        }

        public void CreateHitbox()
        {
            HitboxData newHitbox = new(Vector3.zero, Vector3.zero, Vector3.one, 0f, 0f); 
            m_hitboxData.Add(newHitbox);
        }

        public void CreateEvent()
        {
            EventData newEvent = new EventData();
            m_eventData.Add(newEvent);
        }

        public void CreateEvent(string ID)
        {
            EventData newEvent = new EventData(Data.EventData.EventTypes.Trigger, ID);
            m_eventData.Add(newEvent);
        }

        public void EditEvent(int i, EventData dataToEdit)
        {
            m_eventData[i] = dataToEdit;
        }

        public void EditHitBox(int i, HitboxData dataToEdit)
        {
            m_hitboxData[i] = dataToEdit;
        }

        public void RemoveHitbox(int i) 
        { 
            m_hitboxData.RemoveAt(i);
        }

        public void ClearHitboxes()
        {
            m_hitboxData = new();
        }

        public void EditComboData(ComboInput data)
        {
            m_nextComboInput = data;
        }
    }
}


