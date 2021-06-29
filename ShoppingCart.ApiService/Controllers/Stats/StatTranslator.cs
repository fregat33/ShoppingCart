using ShoppingCart.Common.Entities;

namespace ShoppingCart.ApiService.Controllers.Stats
{
    public static class StatTranslator
    {
        public static StatResponse ToResponse(this StatsEntity statsEntity)
        {
            if (statsEntity == null)
                return null;

            return new StatResponse
                   {
                       AverageCost = statsEntity.StatData.AverageCost,
                       CartsCount = statsEntity.StatData.CartsCount,
                       HasBonusCount = statsEntity.StatData.HasBonusCount,
                       WillEnd10Count = statsEntity.StatData.WillEnd10Count,
                       WillEnd20Count = statsEntity.StatData.WillEnd20Count,
                       WillEnd30Count = statsEntity.StatData.WillEnd30Count,
                   };
        }
    }
}