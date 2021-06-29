using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Common;
using ShoppingCart.Data.Sql.Migrations;

namespace ShoppingCart.Data.Sql.MigrationStrategies
{
    public sealed class ServiceMigrationStrategy : IRuntimeDatabaseMigrationStrategy
    {
        private readonly string _connectionString;
        private const string ConnectionStringName = "CartDatabase";
        private const string DatabaseName = "Cart_Dev";

        public ServiceMigrationStrategy(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(ConnectionStringName);
        }

        public void ApplyMigrations()
        {
            EnsureDatabaseCreated();
            UpdateDatabase();
        }
        
        private void EnsureDatabaseCreated()
        {
            var services = CreateServices(_connectionString.Replace($"Initial Catalog={DatabaseName}", "Initial Catalog=master"));
            using var scope = services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            var script = $@"USE master
GO
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{DatabaseName}')
BEGIN
  CREATE DATABASE {DatabaseName};
END;
GO";
            runner.Processor.Execute(script);
        }

        private void UpdateDatabase()
        {
            var serviceProvider = CreateServices(_connectionString);
            using var scope = serviceProvider.CreateScope();
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
        
        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                   .AddFluentMigratorCore()
                   .ConfigureRunner(
                       rb => rb
                             .AddSqlServer2016()
                             .WithGlobalConnectionString(connectionString)
                             .ScanIn(typeof(InitialCreate).Assembly).For.Migrations())
                   .AddLogging(lb => lb.AddFluentMigratorConsole())
                   .BuildServiceProvider(false);
        }
    }
}