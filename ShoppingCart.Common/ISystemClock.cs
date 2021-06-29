using System;

namespace ShoppingCart.Common
{
    public interface ISystemClock
    {
        DateTimeOffset Now { get; }
        DateTimeOffset UtcNow { get; }
    }
}