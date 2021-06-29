using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Common.Entities;
using ShoppingCart.Common.Extensions;

namespace ShoppingCart.Business.Operations.Actions
{
    public sealed class SendAction
    {
        private const int MaxConcurrency = 5;
        private readonly WebHookSender _webHookSender;
        
        public SendAction(WebHookSender webHookSender)
        {
            _webHookSender = webHookSender;
        }
        
        public async Task<IEnumerable<IGrouping<Guid, (HookEntity hookEntity, bool success, int retries)>>> ExecuteAsync(IReadOnlyCollection<HookEntity> hooks)
        {
            return (await hooks.SelectAsync(h => _webHookSender.TryPostAsync(h), MaxConcurrency))
                .GroupBy(t => t.hookEntity.CartId);
        }
    }
}