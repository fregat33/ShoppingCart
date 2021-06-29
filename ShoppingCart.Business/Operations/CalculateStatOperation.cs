using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShoppingCart.Business.Operations.Actions;
using ShoppingCart.Common.Extensions;

namespace ShoppingCart.Business.Operations
{
    public sealed class CalculateStatOperation : OperationBase
    {
        private readonly DateTimeOffset _fromDate;
        private readonly CalculateStatAction _calculateStatAction;
        
        private static readonly SemaphoreSlim Semaphore = new(1);

        public CalculateStatOperation(
            DateTimeOffset fromDate,
            CalculateStatAction calculateStatAction,
            ILoggerFactory loggerFactory): base(loggerFactory, Semaphore)
        {
            _fromDate = fromDate;
            _calculateStatAction = calculateStatAction;
        }

        protected override Task OnExecuteAsync()
        {
            return _calculateStatAction.ExecuteAsync(_fromDate).ExecuteWithTimingsLogAsync(nameof(CalculateStatAction), Logger);
        }
    }
}