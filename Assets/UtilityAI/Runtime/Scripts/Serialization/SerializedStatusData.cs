using UnityEngine;

namespace Stirge.UtilityAI
{
    [CreateAssetMenu]
    public class SerializedStatusData : ScriptableObject
    {
        [SerializeField, Range(0f, 5f)] private float m_scaling = 1f;
        [SerializeField] private StatusStackType m_stackType;
        [SerializeField] private StatusDurationType m_durationType;
        [SerializeField] private string m_displayName;
        [SerializeField, Min(1)] private int m_maxStacks;
        [SerializeField] private Condition[] m_conditions = new Condition[0];

        public StatusData CreateRuntimeStatusData()
        {
            return new StatusData(m_scaling, m_stackType, m_durationType, m_displayName, m_maxStacks, m_conditions);
        }
    }
}
