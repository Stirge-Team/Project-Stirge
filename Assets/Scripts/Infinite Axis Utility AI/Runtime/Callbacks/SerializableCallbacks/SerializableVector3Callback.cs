using UnityEngine;
using System;

namespace Stirge.UtilityAI.Callbacks
{
    [Serializable]
    public class SerializableVector3Callback : CallbackBinding<Vector3>
    {
        [SerializeField] private Vector3Callback m_callback;

        public override SerializableCallback<Vector3> callback => m_callback;

        [Serializable]
        private class Vector3Callback : SerializableCallback<Vector3> { }
    }
}
