using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Common;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Business.Operations.Actions
{
    public sealed class InitCleanAction
    {
        private readonly IHooksRepository _hooksRepository;
        private readonly SendAction _sendAction;
        private readonly ChangeHookStateAction _changeHookStateAction;

        public InitCleanAction(IHooksRepository hooksRepository, 
                               SendAction sendAction,
                               ChangeHookStateAction changeHookStateAction)
        {
            _hooksRepository = hooksRepository;
            _sendAction = sendAction;
            _changeHookStateAction = changeHookStateAction;
        }
        
        public async Task ExecuteAsync()
        {
            var readByState = await _hooksRepository.ReadByStateAsync(HookState.Activated);
            var hooksByCart = await _sendAction.ExecuteAsync(readByState.ToArray());
            await _changeHookStateAction.ExecuteAsync(hooksByCart);
        }
    }
}