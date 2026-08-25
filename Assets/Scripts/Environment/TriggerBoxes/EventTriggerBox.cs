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
        protected override void EnterFunc(Collider collider)
        {
            base.EnterFunc(collider);
            m_enterEvent.Invoke(collider);
        }
        protected override void StayFunc(Collider collider)
        {
            base.StayFunc(collider);
            m_stayEvent.Invoke(collider);
        }
        protected override void ExitFunc(Collider collider)
        {
            base.ExitFunc(collider);
            m_exitEvent.Invoke(collider);
        }
    }
}