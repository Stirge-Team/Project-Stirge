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

        public override void DetermineValue()
        {
            m_x.DetermineValue();
            m_y.DetermineValue();
            m_z.DetermineValue();
            m_value = new(m_x.Value, m_y.Value, m_z.Value);
        }
    }
}
