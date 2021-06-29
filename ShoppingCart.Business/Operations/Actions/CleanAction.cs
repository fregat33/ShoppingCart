using System;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Business.Operations.Actions
{
    public sealed class CleanAction
    {
        private readonly ICartRepository _cartRepository;
        private readonly IHooksRepository _hooksRepository;
        private readonly SendAction _sendActionFactory;
        private readonly ChangeHookStateAction _changeHookStateAction;

        public CleanAction(ICartRepository cartRepository, IHooksRepository hooksRepository, SendAction sendActionFactory, ChangeHookStateAction changeHookStateAction)
        {
            _cartRepository = cartRepository;
            _hooksRepository = hooksRepository;
            _sendActionFactory = sendActionFactory;
            _changeHookStateAction = changeHookStateAction;
        }
        
        public async Task ExecuteAsync(DateTimeOffset toDate)
        {
            var entityIds = (await _cartRepository.ReadCartIdsByPeriodAsync(fromDate: null, toDate: toDate)).ToArray();
            var hooks = (await _hooksRepository.ReadByCartIdsAsync(entityIds)).ToArray();

            await _hooksRepository.SetActivateAsync(hooks.Where(h => h.State == HookState.Created).Select(h => h.CartId).Distinct().ToArray());
           
            var hooksByCart =  await _sendActionFactory.ExecuteAsync(hooks);
            await _changeHookStateAction.ExecuteAsync(hooksByCart);
        }
    }
}