using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ShoppingCart.Common.Extensions
{
    public static class TaskExtensions
    {
        public static async Task ExecuteWithTimingsLogAsync(
            this Task task,
            string taskName,
            ILogger logger)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await task;
            }
            finally
            {
                sw.Stop();
                logger.LogInformation("Task duration {TaskName}: {ElapsedMilliseconds} (ms)", taskName, sw.ElapsedMilliseconds);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DoNotWait(this Task task)
        {
        }
    }
}