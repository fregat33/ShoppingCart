using System;
using System.Text.Json;

namespace ShoppingCart.Common.Entities
{
    public record StatsEntity
    {
        private readonly Lazy<StatData> _lazyStatData;
        public int Id { get; }
        public string Data { get; }
        public StatData StatData => _lazyStatData.Value;
        public DateTimeOffset CreationDate { get; }

        public StatsEntity()
        {
            
        }

        public StatsEntity(int id, string data, DateTimeOffset creationDate)
        {
            _lazyStatData = new Lazy<StatData>(() => JsonSerializer.Deserialize<StatData>(data));
            Id = id;
            Data = data;
            CreationDate = creationDate;
        }
        
        public StatsEntity(int id, StatData data, DateTimeOffset creationDate)
        {
            _lazyStatData = new Lazy<StatData>(() => data);
            Id = id;
            Data = JsonSerializer.Serialize(data);
            CreationDate = creationDate;
        }
    }
}