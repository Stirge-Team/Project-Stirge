using System;

namespace Stirge.UtilityAI.Builders
{
    using Action = Core.Action;

    public interface IActionBuilder
    {
        Type actionType { get; }

        Action Build();
    }
}
