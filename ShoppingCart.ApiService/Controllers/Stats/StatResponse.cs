namespace ShoppingCart.ApiService.Controllers.Stats
{
    public class StatResponse
    {
        /// <summary>
        /// Средняя стоимость корзины
        /// </summary>
        public decimal AverageCost { get; set; }
        /// <summary>
        /// Число корзин
        /// </summary>
        public int CartsCount { get; set; }
        /// <summary>
        /// Число корзин содержащих баллы
        /// </summary>
        public int HasBonusCount { get; set; }
        /// <summary>
        /// Корзин закочится через 10 дней
        /// </summary>
        public int WillEnd10Count { get; set; }
        /// <summary>
        /// Корзин закочится через 20 дней
        /// </summary>
        public int WillEnd20Count { get; set; }
        /// <summary>
        /// Корзин закочится через 30 дней
        /// </summary>
        public int WillEnd30Count { get; set; }
    }
}