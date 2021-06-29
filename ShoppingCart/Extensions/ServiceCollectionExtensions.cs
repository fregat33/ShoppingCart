using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShoppingCart.Business;
using ShoppingCart.Business.Contracts;
using ShoppingCart.Business.Operations.Actions;
using ShoppingCart.Common;
using ShoppingCart.Data.Sql;
using ShoppingCart.Data.Sql.MigrationStrategies;
using ShoppingCart.Data.Sql.Repositories;
using ShoppingCart.HostedServices;
using ShoppingCart.Settings;

namespace ShoppingCart.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string ConnectionName = "CartDatabase";

        public static IServiceCollection AddShoppingCartDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<InitHostedService>()
                    .AddHostedService<StatSchedulerHostedService>();
            
            services.AddAutoFactory<ICalculateStatOperation>()
                    .AddAutoFactory<ICleanOperationFactory>();

            services.AddTransient<CalculateStatAction>()
                    .AddTransient<ChangeHookStateAction>()
                    .AddTransient<CleanAction>()
                    .AddTransient<InitCleanAction>()
                    .AddTransient<SendAction>();

            services.AddOptions()
                    .AddOptions<StatSchedulerServiceSettings>().Bind(configuration.GetSection(nameof(ShoppingCartSettings.StatSchedulerServiceSettings)));

            var connectionString = configuration.GetConnectionString(ConnectionName);
            services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString))
                    .AddSingleton<IRuntimeDatabaseMigrationStrategy, ServiceMigrationStrategy>();

            services.AddSingleton<ICartRepository, CartRepository>()
                    .AddSingleton<IHooksRepository, HooksRepository>()
                    .AddSingleton<IStatsRepository, StatsRepository>()
                    .AddSingleton<WebHookSender>();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<ISystemClock, SystemClock>();

            return services;
        }

        public static IServiceCollection AddAutoFactory<TFactory>(this IServiceCollection services)
            where TFactory : class
        {
            services.AddSingleton(CreateFactory<TFactory>);
            return services;
        }

        private static TFactory CreateFactory<TFactory>(IServiceProvider serviceProvider)
            where TFactory : class
        {
            var generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget<TFactory>(
                new FactoryInterceptor(serviceProvider));
        }

        private class FactoryInterceptor : IInterceptor
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly ConcurrentDictionary<MethodInfo, ObjectFactory> _factories;

            public FactoryInterceptor(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
                _factories = new ConcurrentDictionary<MethodInfo, ObjectFactory>();
            }

            public void Intercept(IInvocation invocation)
            {
                var factory = _factories.GetOrAdd(invocation.Method, CreateFactory);
                invocation.ReturnValue = factory(_serviceProvider, invocation.Arguments);
            }

            private ObjectFactory CreateFactory(MethodInfo method)
            {
                return ActivatorUtilities.CreateFactory(
                    method.ReturnType,
                    method.GetParameters().Select(p => p.ParameterType).ToArray());
            }
        }
    }
}