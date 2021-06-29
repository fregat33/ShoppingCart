using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShoppingCart.Business.Operations.Actions;
using ShoppingCart.Common.Extensions;

namespace ShoppingCart.Business.Operations
{
    public sealed class CleanOperation: OperationBase
    {
        private readonly DateTimeOffset? _toDate;
        
        private readonly CleanAction _cleanAction;
        private readonly InitCleanAction _initCleanAction;
        
        private static readonly SemaphoreSlim Semaphore = new(1);

        public CleanOperation(
            DateTimeOffset? toDate,
            CleanAction cleanAction,
            InitCleanAction initCleanAction,
            ILoggerFactory loggerFactory): base(loggerFactory, Semaphore)
        {
            _toDate = toDate;
            _cleanAction = cleanAction;
            _initCleanAction = initCleanAction;
        }

        protected override Task OnExecuteAsync()
        {
            return _toDate.HasValue 
                       ? _cleanAction.ExecuteAsync(_toDate.Value).ExecuteWithTimingsLogAsync(nameof(CleanAction), Logger) 
                       : _initCleanAction.ExecuteAsync().ExecuteWithTimingsLogAsync(nameof(InitCleanAction), Logger);
        }
    }
}