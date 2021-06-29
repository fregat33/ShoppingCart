using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShoppingCart.Common.Entities;

namespace ShoppingCart.Business
{
    public class WebHookSender
    {
        private readonly ILogger<WebHookSender> _logger;
        private readonly HttpClient _client;

        public WebHookSender(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WebHookSender>();
            _client = new HttpClient();
        }

        public async Task<(HookEntity hookEntity, bool success, int retries)> TryPostAsync(HookEntity hookEntity, int maxRetriesCount = 3)
        {
            var retriesCount = 0;
            Exception lastException = null;
            while (++retriesCount <= maxRetriesCount)
            {
                try
                {
                    var content = JsonContent.Create(hookEntity.Payload);
                    var response = await _client.PostAsync(hookEntity.Url, content);
                    if (response.IsSuccessStatusCode)
                        return (hookEntity, response.IsSuccessStatusCode, retriesCount);
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if(retriesCount < maxRetriesCount)
                        await Task.Delay(100 * retriesCount);
                }
            }

            LogError(hookEntity, retriesCount, lastException);

            return (hookEntity, false, retriesCount);
        }

        private void LogError(HookEntity hookEntity, int retriesCount, Exception lastException = null)
        {
            if (lastException != null)
            {
                _logger.LogError(
                    lastException,
                    "Error on sending webhook (id: {id}) after {retriesCount} tries! {Uri}",
                    hookEntity.Id,
                    retriesCount,
                    hookEntity.Url);
            }
            else
            {
                _logger.LogError(
                    "Error on sending webhook (id: {id}) after {retriesCount} tries! {Uri}",
                    hookEntity.Id,
                    retriesCount,
                    hookEntity.Url);
            }
        }
    }
}