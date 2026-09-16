using UnityEngine;

namespace Stirge.Tools
{
    public abstract class RandomField<T> where T : struct
    {
        [SerializeField] protected T m_value;
        public T Value => m_value;

        public abstract void DetermineValue();
    }
}
