using System;

namespace ShoppingCart.Common
{
    public class SystemClock : ISystemClock
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}