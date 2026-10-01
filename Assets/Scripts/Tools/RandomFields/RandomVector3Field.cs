using UnityEngine;

namespace Stirge.Tools
{
    [System.Serializable]
    public class RandomVector3Field : RandomField<Vector3>
    {
        [SerializeField] private RandomFloatField m_x;
        [SerializeField] private RandomFloatField m_y;
        [SerializeField] private RandomFloatField m_z;

        public RandomVector3Field()
        {
            m_x = new();
            m_y = new();
            m_z = new();
        }
        public RandomVector3Field(float value)
        {
            m_x = new(value);
            m_y = new(value);
            m_z = new(value);
        }
        public RandomVector3Field(float x, float y, float z)
        {
            m_x = new(x);
            m_y = new(y);
            m_z = new(z);
        }
        public RandomVector3Field(Vector2 range)
        {
            m_x = new(range);
            m_y = new(range);
            m_z = new(range);
        }
        public RandomVector3Field(Vector2 xRange, Vector2 yRange, Vector2 zRange)
        {
            m_x = new(xRange);
            m_y = new(yRange);
            m_z = new(zRange);
        }

        public override void DetermineValue()
        {
            m_x.DetermineValue();
            m_y.DetermineValue();
            m_z.DetermineValue();
            m_value = new(m_x.Value, m_y.Value, m_z.Value);
        }
    }
}
