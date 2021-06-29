using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShoppingCart.Business.Contracts;
using ShoppingCart.Common;
using ShoppingCart.Common.Extensions;
using ShoppingCart.Settings;

namespace ShoppingCart.HostedServices
{
    public class StatSchedulerHostedService : IHostedService
    {
        private const int CartValidityPeriodDays = -30;
        
        private readonly IOptionsMonitor<StatSchedulerServiceSettings> _statSchedulerServiceOptions;
        private readonly ICalculateStatOperation _calculateStatOperationFactory;
        private readonly ICleanOperationFactory _cleanOperationFactory;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<StatSchedulerHostedService> _logger;
        private readonly Timer _timer;

        public StatSchedulerHostedService(
            IOptionsMonitor<StatSchedulerServiceSettings> statSchedulerServiceOptions,
            ICalculateStatOperation calculateStatOperationFactoryFactory,
            ICleanOperationFactory cleanOperationFactory,
            ISystemClock systemClock,
            ILoggerFactory loggerFactory)
        {
            _statSchedulerServiceOptions = statSchedulerServiceOptions;
            
            _statSchedulerServiceOptions.OnChange(SettingsChanged);
            _calculateStatOperationFactory = calculateStatOperationFactoryFactory;
            _systemClock = systemClock;
            _logger = loggerFactory.CreateLogger<StatSchedulerHostedService>();
            _cleanOperationFactory = cleanOperationFactory;
            _timer = new Timer(RepeatableTask);
        }

        private void SettingsChanged(StatSchedulerServiceSettings settings)
        {
            var period = settings.Interval;
            var dueTime = GetDueTime(settings.StartAt, _systemClock.UtcNow.TimeOfDay);
            _timer.Change(dueTime, period);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            SettingsChanged(_statSchedulerServiceOptions.CurrentValue);
            return Task.CompletedTask;
        }

        private void RepeatableTask(object state)
        {
            Task.Run(ExecuteAsync);
        }

        private  Task ExecuteAsync()
        {
            var startDate = _systemClock.UtcNow;
            _logger.LogInformation("Stat collection started!");
            var lastDate = startDate.AddDays(CartValidityPeriodDays);
            var calculateTask = _calculateStatOperationFactory.Create(lastDate).ExecuteAsync();
            var clearTask = _cleanOperationFactory.Create(lastDate).ExecuteAsync();
            return Task.WhenAll(calculateTask, clearTask).ExecuteWithTimingsLogAsync(nameof(RepeatableTask), _logger);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            await _timer.DisposeAsync();
        }

        private static TimeSpan GetDueTime(TimeSpan startAt, TimeSpan current)
        {
            if (current > startAt)
                return TimeSpan.FromDays(1) - current + startAt;

            if (startAt > current)
                return startAt - current;

            return TimeSpan.FromSeconds(5);
        }
    }
}