using UnityEngine;

namespace Stirge.Tools
{
    [System.Serializable]
    public class RandomVector2Field : RandomField<Vector2>
    {
        [SerializeField] private RandomFloatField m_x;
        [SerializeField] private RandomFloatField m_y;

        public RandomVector2Field()
        {
            m_x = new();
            m_y = new();
        }

        public override void DetermineValue()
        {
            m_x.DetermineValue();
            m_y.DetermineValue();
            m_value = new(m_x.Value, m_y.Value);
        }
    }
}
