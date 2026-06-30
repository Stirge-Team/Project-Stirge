using System;
using UnityEngine;

namespace Stirge.UtilityAI.Callbacks
{
    [Serializable]
    public class SerializableFloatCallback : CallbackBinding<float>
    {
        [SerializeField] private FloatCallback m_callback;

        public override SerializableCallback<float> callback => m_callback;

        [Serializable]
        private class FloatCallback : SerializableCallback<float> { }
    }
}
