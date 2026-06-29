using System;
using UnityEngine;
using Zor.SimpleBlackboard.Serialization;

namespace Stirge.UtilityAI.Callbacks
{
    [Serializable]
    public class BoolCallback : SerializableCallback<bool> { }
    [Serializable]
    public class FloatCallback : SerializableCallback<float> { }
    [Serializable]
    public class Vector3Callback : SerializableCallback<Vector3> { }

    public sealed class BoolCallbackSerializedTable : ClassSerializedValueSerializedTable<BoolCallback> { }
    public sealed class FloatCallbackSerializedTable : ClassSerializedValueSerializedTable<FloatCallback> { }
    public sealed class Vector3CallbackSerializedTable : ClassSerializedValueSerializedTable<Vector3Callback> { }

}
