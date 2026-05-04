using UnityEngine;

namespace Stirge.Tools
{
    [System.Serializable]
    public class RandomFloatField
    {
        public RandomFloatField()
        {
            m_isRandom = false;
            m_range = new();
            m_value = 1f;
        }
        
        [SerializeField] private bool m_isRandom;
        [SerializeField] private Vector2 m_range;
        [SerializeField] private float m_value;

        public float Value => m_value;

        public void DetermineValue()
        {
            if (m_isRandom)
            {
                m_value = Random.Range(m_range.x, m_range.y);
            }
        }
    }
}
