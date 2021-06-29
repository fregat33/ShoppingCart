using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Business.Operations.Actions
{
    public sealed class ChangeHookStateAction
    {
        private readonly IHooksRepository _hooksRepository;
        private readonly ICartRepository _cartRepository;

        public ChangeHookStateAction(IHooksRepository hooksRepository, ICartRepository cartRepository)
        {
            _hooksRepository = hooksRepository;
            _cartRepository = cartRepository;
        }

        public async Task ExecuteAsync(IEnumerable<IGrouping<Guid, (HookEntity hookEntity, bool success, int retries)>> hooksGroupedByCartIds)
        {
            foreach (var cartHooks in hooksGroupedByCartIds)
            {
                var cartHooksArray = cartHooks.ToArray();
                if (!await AllHooksSentSuccessfully(cartHooks.Key, cartHooksArray))
                    await UpdateHookStates(cartHooksArray);
            }
        }

        private async Task UpdateHookStates((HookEntity hookEntity, bool success, int retries)[] cartHooks)
        {
            foreach (var (hookEntity, success, retries) in cartHooks)
            {
                if (success)
                    await _hooksRepository.DeleteAsync(hookEntity.Id);
                else
                    await _hooksRepository.SetFailedAsync(hookEntity.Id, retries);
            }
        }

        private async Task<bool> AllHooksSentSuccessfully(Guid cartId, (HookEntity hookEntity, bool success, int retries)[] cartHooks)
        {
            if (!cartHooks.All(g => g.success))
                return false;

            await _cartRepository.DeleteAsync(cartId);
            return true;
        }
    }
}