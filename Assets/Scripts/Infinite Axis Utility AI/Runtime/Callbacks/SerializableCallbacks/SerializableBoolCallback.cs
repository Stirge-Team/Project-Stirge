using System;
using UnityEngine;

namespace Stirge.UtilityAI.Callbacks
{
    [Serializable]
    public class SerializableBoolCallback : CallbackBinding<bool>
    {
        [SerializeField] private BoolCallback m_callback;

        public override SerializableCallback<bool> callback => m_callback;

        [Serializable]
        private class BoolCallback : SerializableCallback<bool> { }
    }
}
