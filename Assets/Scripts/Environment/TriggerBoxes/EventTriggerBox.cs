using UnityEngine;
using UnityEngine.Events;

namespace Stirge.Environment
{
    public class EventTriggerBox : SimpleTriggerBox
    {
        [SerializeField]
        private UnityEvent<Collider> m_enterEvent;
        [SerializeField]
        private UnityEvent<Collider> m_stayEvent;
        [SerializeField]
        private UnityEvent<Collider> m_exitEvent;
        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);
            m_enterEvent.Invoke(collider);
        }
        public override void OnTriggerStay(Collider collider)
        {
            base.OnTriggerStay(collider);
            m_stayEvent.Invoke(collider);
        }
        public override void OnTriggerExit(Collider collider)
        {
            base.OnTriggerExit(collider);
            m_exitEvent.Invoke(collider);
        }
    }
}