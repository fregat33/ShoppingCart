using System;

namespace ShoppingCart.Common.Entities
{
    public record HookEntity
    {
        public int Id { get; set; }
        public Guid CartId { get; set; }
        public string Url { get; set; }
        public string Payload { get; set; }
        public HookState State { get; set; }
        public int RetriesCount { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }
}