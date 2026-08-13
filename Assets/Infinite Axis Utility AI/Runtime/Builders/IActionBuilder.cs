using System;

namespace Stirge.InfiniteAxis.Builders
{
    using Action = Core.Action;

    public interface IActionBuilder
    {
        Type actionType { get; }

        Action Build();
    }
}
