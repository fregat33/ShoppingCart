using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Common
{
    public interface IStatsRepository
    {
        Task<StatsEntity> CreateAsync(StatData data, DateTimeOffset creationDate);
        Task<IReadOnlyCollection<StatsEntity>> ReadByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate);
    }
}