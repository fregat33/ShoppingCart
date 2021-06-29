using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Common
{
    public interface IHooksRepository
    {
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<int>> SetActivateAsync(IReadOnlyCollection<Guid> cartIds);
        Task SetFailedAsync(int id, int retries);
        Task<HookEntity> CreateAsync(Guid requestCartId, string requestUrl, string payload, DateTimeOffset creationDate);
        Task<IEnumerable<HookEntity>> ReadByStateAsync(HookState state);
        Task<IEnumerable<HookEntity>> ReadByCartIdAsync(Guid cartId);
        Task<IEnumerable<HookEntity>> ReadByCartIdsAsync(IReadOnlyCollection<Guid> cartIds);
    }
}