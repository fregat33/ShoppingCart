namespace ShoppingCart.Common.Entities
{
    public record StatData
    {
        public int CartsCount { get; set; }
        public int HasBonusCount { get; set; }
        public int WillEnd10Count { get; set; }
        public int WillEnd20Count { get; set; }
        public int WillEnd30Count { get; set; }
        public decimal AverageCost { get; set; }
    }
}