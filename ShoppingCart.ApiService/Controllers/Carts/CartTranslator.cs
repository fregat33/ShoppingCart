using System;
using System.Linq;
using System.Text.Json;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.ApiService.Controllers.Carts
{
    public static class CartTranslator
    {
        public static CartEntity ToEntity(this CartRequest request, Guid id, DateTimeOffset creationDate)
        {
            if (request == null)
                return null;

            var productItems = request.Items.Select(
                i => new Product
                     {
                         // all values from request are required
                         Cost = i.Cost.Value, 
                         Count = i.Count.Value,
                         ForBonusPoints = i.ForBonusPoints.Value,
                         Id = i.Id.Value,
                         Name = i.Name
                     }).ToArray();

            return new CartEntity(id, productItems, creationDate);
        }

        public static CartResponse ToResponse(this CartEntity entity)
        {
            if (entity == null)
                return null;

            return new CartResponse
                   {
                       Id = entity.Id,
                       Cost = entity.Cost,
                       CreationDate = entity.CreationDate,
                       Items = JsonSerializer.Deserialize<Product[]>(entity.Items),
                       HasBonusPoints = entity.HasBonusPoints
                   };
        }
    }
}