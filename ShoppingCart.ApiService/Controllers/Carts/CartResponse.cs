using System;
using System.Collections.Generic;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.ApiService.Controllers.Carts
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public IReadOnlyCollection<Product> Items { get; set; }
        public decimal Cost { get; set; }
        public bool HasBonusPoints { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }
}