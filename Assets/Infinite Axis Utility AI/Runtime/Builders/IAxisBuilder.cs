using System;
using UnityEngine;

namespace Stirge.InfiniteAxis.Builders
{
    using Core;

    public interface IAxisBuilder
    {
        Type axisType { get; }

        Axis Build();

        bool AreEqual(object[] parameters);
    }
}
