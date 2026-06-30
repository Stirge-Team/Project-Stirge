using System;
using UnityEngine;

namespace Stirge.UtilityAI.Callbacks
{
    public abstract class CallbackBinding<T> : CallbackBinding_Base
    {
        public abstract SerializableCallback<T> callback { get; }

        public override Type valueType => typeof(T);

        public T GetValue()
        {
            return callback.Invoke();
        }

        public override object GetObjectValue()
        {
            return callback.Invoke();
        }
    }
}