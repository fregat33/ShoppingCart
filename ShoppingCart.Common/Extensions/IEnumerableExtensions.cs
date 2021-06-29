using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static async Task<TResult[]> SelectAsync<TInput, TResult>(
            this IEnumerable<TInput> items,
            Func<TInput, Task<TResult>> converter,
            int? maxConcurrency = null)
        {
            if (!maxConcurrency.HasValue)
                return await Task.WhenAll(items.Select(converter));
            var oneAtATime = new SemaphoreSlim(maxConcurrency.Value, maxConcurrency.Value);
            return await Task.WhenAll(items.Select((Func<TInput, Task<TResult>>) (s => ProcessResultAsync(s, converter, oneAtATime))).ToArray());
        }
        
        private static async Task<TResult> ProcessResultAsync<TSource, TResult>(
            TSource item,
            Func<TSource, Task<TResult>> taskSelector,
            SemaphoreSlim oneAtATime)
        {
            await oneAtATime.WaitAsync().ConfigureAwait(false);
            try
            {
                return await taskSelector(item).ConfigureAwait(false);
            }
            finally
            {
                oneAtATime.Release();
            }
        }
    }
}