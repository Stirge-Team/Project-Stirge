using System;
using UnityEngine;

namespace Stirge.UtilityAI
{
    public interface IActionBuilder
    {
        Type actionType { get; }

        Action Build();
    }
}
