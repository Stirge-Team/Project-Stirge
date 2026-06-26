using UnityEngine;
using System.Collections.Generic;

// https://discussions.unity.com/t/solved-how-to-serialize-dictionary-with-unity-serialization-system/71474/4

namespace Stirge.Tools
{
    [System.Serializable]
    public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver where TValue : class
    {
        [SerializeField] private List<TKey> m_keys = new();
        [SerializeField] private List<TValue> m_values = new();

        // Save dictionary to lsts
        public void OnBeforeSerialize()
        {
            m_keys.Clear();
            m_values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                m_keys.Add(pair.Key);
                m_values.Add(pair.Value);
            }
        }

        // Load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            // likely there are null values
            if (m_keys.Count != m_values.Count)
            {
                Debug.LogError(string.Format($"There are {m_keys.Count} keys and {m_values.Count} values after deserialization. Make sure that both key and value types are serializable."));

                while (m_values.Count < m_keys.Count)
                {
                    m_values.Add(null);
                }
            }

            for (int i = 0; i < m_keys.Count; i++)
            {
                this.Add(m_keys[i], m_values[i]);
            }
        }
    }
}
