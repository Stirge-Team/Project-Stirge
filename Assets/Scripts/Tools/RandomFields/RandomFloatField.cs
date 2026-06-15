using UnityEngine;

namespace Stirge.Tools
{
    [System.Serializable]
    public class RandomFloatField : RandomField<float>
    {
        [SerializeField] private Vector2 m_range;
        [SerializeField] private bool m_isRandom;

        public RandomFloatField()
        {
            m_isRandom = false;
            m_range = new();
            m_value = 1f;
        }

        public override void DetermineValue()
        {
            if (m_isRandom)
            {
                m_value = Random.Range(m_range.x, m_range.y);
            }
        }
    }
}
