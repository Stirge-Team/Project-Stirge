using UnityEngine;

namespace Stirge.Environment
{
    /// <summary>
    /// Reenables selected trigger boxes when entered
    /// </summary>
    public class ReTriggerBox : SimpleTriggerBox
    {
        [System.Serializable]
        private struct ReTriggerData
        {
            public SimpleTriggerBox _trigger;
            public SelectTriggerEvent _selectTrigger;
            public void Reenable()
            {
                _trigger.ReenableTriggers(_selectTrigger);
            }
        }
        [Header("Selected Triggers")]
        [SerializeField]
        private ReTriggerData[] m_reTriggerData;

        protected override void EnterFunc(Collider collider)
        {
            base.EnterFunc(collider);

            foreach(var trig in m_reTriggerData)
            {
                trig.Reenable();
            }
        }
    }
}