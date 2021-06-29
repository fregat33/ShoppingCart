using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ShoppingCart.Common.Entities
{
    public record CartEntity
    {
        private readonly Lazy<IReadOnlyCollection<Product>> _lazyProductItems;
        public IReadOnlyCollection<Product> ProductItems => _lazyProductItems.Value;
        
        public Guid Id { get; set; }
        public decimal Cost { get; set; }
        public string Items { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public bool HasBonusPoints { get; set; }

        public CartEntity()
        {
            
        }

        public CartEntity(Guid id, decimal cost, string items, DateTimeOffset creationDate, bool hasBonusPoints)
        {
            _lazyProductItems = new Lazy<IReadOnlyCollection<Product>>(() => JsonSerializer.Deserialize<IReadOnlyCollection<Product>>(items));
            Id = id;
            Cost = cost;
            Items = items;
            CreationDate = creationDate;
            HasBonusPoints = hasBonusPoints;
        }

        public CartEntity(Guid id, IReadOnlyCollection<Product> productItems, DateTimeOffset creationDate)
        {
            _lazyProductItems = new Lazy<IReadOnlyCollection<Product>>(() => productItems);
            Id = id;
            Items = JsonSerializer.Serialize(productItems);
            Cost = productItems.Sum(p => p.Cost);
            HasBonusPoints = productItems.Any(p => p.ForBonusPoints);
            CreationDate = creationDate;
        }
        
        public void Deconstruct(out Guid id, out string items, out decimal cost, out bool hasBonusProducts, out DateTimeOffset creationDate) => 
            (id, items, cost, hasBonusProducts, creationDate) = (Id, Items, Cost, HasBonusPoints, CreationDate);
    }
}