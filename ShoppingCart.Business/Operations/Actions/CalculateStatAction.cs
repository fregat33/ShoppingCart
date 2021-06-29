using System;
using System.Threading.Tasks;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Business.Operations.Actions
{
    public sealed class CalculateStatAction
    {
        private readonly IStatsRepository _statsRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ISystemClock _systemClock;

        public CalculateStatAction(IStatsRepository statsRepository, ICartRepository cartRepository, ISystemClock systemClock)
        {
            _statsRepository = statsRepository;
            _cartRepository = cartRepository;
            _systemClock = systemClock;
        }

        public async Task ExecuteAsync(DateTimeOffset fromDate)
        {
            var cartsCountTask = _cartRepository.CountFromDateAsync(fromDate);
            var willEnd10CountTask = _cartRepository.CountFromDateAsync(fromDate.AddDays(10));
            var willEnd20CountTask = _cartRepository.CountFromDateAsync(fromDate.AddDays(20));
            var willEnd30CountTask = _cartRepository.CountFromDateAsync(fromDate.AddDays(30));
            var averageCostTask = _cartRepository.AvgCostFromDateAsync(fromDate);
            var hasBonusCountTask = _cartRepository.CountHasBonusFromDateAsync(fromDate);

            await Task.WhenAll(cartsCountTask, willEnd10CountTask, willEnd20CountTask, willEnd30CountTask, averageCostTask, hasBonusCountTask);

            var statData = new StatData
                           {
                               CartsCount = cartsCountTask.Result,
                               AverageCost = averageCostTask.Result,
                               HasBonusCount = hasBonusCountTask.Result,
                               WillEnd10Count = willEnd10CountTask.Result,
                               WillEnd20Count = willEnd20CountTask.Result,
                               WillEnd30Count = willEnd30CountTask.Result,
                           };

            await _statsRepository.CreateAsync(statData, _systemClock.UtcNow);
        }
    }
}