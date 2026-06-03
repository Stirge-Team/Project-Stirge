using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface IAxisBuilder
    {
        Type axisType { get; }

        Axis Build();

        bool AreEqual(object[] parameters);
    }
}
