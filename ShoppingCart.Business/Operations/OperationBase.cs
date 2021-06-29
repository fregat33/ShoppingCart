using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ShoppingCart.Business.Operations
{
    public abstract class OperationBase
    {
        protected readonly ILogger Logger;
        private readonly SemaphoreSlim _semaphore;
        private const int MaxRetryCount = 3;

        protected OperationBase(ILoggerFactory loggerFactory, SemaphoreSlim semaphore)
        {
            Logger = loggerFactory.CreateLogger(GetType());
            _semaphore = semaphore;
        }

        public async Task ExecuteAsync()
        {
            await _semaphore.WaitAsync();
            var retriesCount = 0;

            try
            {
                while (++retriesCount <= MaxRetryCount)
                {
                    try
                    {
                        await OnExecuteAsync();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(
                            ex,
                            "Error on executing operation (type: {OperationName}) after {RetriesCount} tries!",
                            GetType().Name,
                            retriesCount);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected abstract Task OnExecuteAsync();
    }
}