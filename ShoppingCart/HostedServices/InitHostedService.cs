using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoppingCart.Business.Contracts;
using ShoppingCart.Common;
using ShoppingCart.Common.Extensions;

namespace ShoppingCart.HostedServices
{
    public class InitHostedService : IHostedService
    {
        private readonly IRuntimeDatabaseMigrationStrategy _runtimeDatabaseMigrationStrategy;
        private readonly ICleanOperationFactory _cleanOperationFactory;
        private readonly ILogger<InitHostedService> _logger;

        public InitHostedService(
            IRuntimeDatabaseMigrationStrategy runtimeDatabaseMigrationStrategy,
            ICleanOperationFactory cleanOperationFactory, 
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<InitHostedService>();
            _runtimeDatabaseMigrationStrategy = runtimeDatabaseMigrationStrategy;
            _cleanOperationFactory = cleanOperationFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service starting...");
            _runtimeDatabaseMigrationStrategy.ApplyMigrations();
            _cleanOperationFactory.Create().ExecuteAsync().DoNotWait();
            _logger.LogInformation("Service started!");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}