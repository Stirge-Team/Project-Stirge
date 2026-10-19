using UnityEngine;

namespace Stirge.Combat.Hitboxes
{
    [System.Serializable]
    public class StatusInflictData
    {
        [Tooltip("The index of the Status on its Action to be inflicted.")]
        [SerializeField] private int m_statusActionIndex;
        [Tooltip("If a target is hit multiple times, whether or not to inflict that Status multiple times.")]
        [SerializeField] private bool m_inflictOnlyOnce;
        [Tooltip("If the Status is stackable, how many stacks to inflict.")]
        [SerializeField] private int m_stacks;
        [Tooltip("If the Status is Timed, how long it should last.")]
        [SerializeField] private float m_duration;

        public int StatusActionIndex => m_statusActionIndex;
        public bool InflictOnlyOnce => m_inflictOnlyOnce;
        public int Stacks => m_stacks;
        public float Duration => m_duration;
    }
}
