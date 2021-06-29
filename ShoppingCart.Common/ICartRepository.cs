using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Common
{
    public interface ICartRepository
    {
        Task<CartEntity> CreateAsync(
            decimal cost,
            string items,
            bool hasBonusPoints,
            DateTimeOffset creationDate);
        
        Task<CartEntity> ReadByIdAsync(Guid id);
        Task<IEnumerable<CartEntity>> ReadCartsByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate);
        Task<IEnumerable<Guid>> ReadCartIdsByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate);
        Task<bool> UpdateAsync(CartEntity cartEntity);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Guid>> DeleteByPeriodAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate);

        Task<int> AvgCostFromDateAsync(DateTimeOffset fromDate);
        Task<int> CountFromDateAsync(DateTimeOffset fromDate);
        Task<int> CountHasBonusFromDateAsync(DateTimeOffset fromDate);
    }
}