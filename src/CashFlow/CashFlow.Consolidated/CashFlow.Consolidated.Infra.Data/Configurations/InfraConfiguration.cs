
using CashFlow.Consolidated.Domain.SharedKernel.Events;
using CashFlow.Consolidated.Infra.Data.Contracts;
using CashFlow.Consolidated.Infra.Data.Gateways;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Refit;
using StackExchange.Redis;

namespace CashFlow.Consolidated.Infra.Data.DatabaseConfigurations
{
    public static class InfraConfiguration
    {
        public static IServiceCollection ConfigureInfra(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

            services.AddSingleton(sp =>
            {
                var factory = new ConnectionFactory() { HostName = configuration.GetConnectionString("Rabbitmq") };
                return factory.CreateConnection();
            });

            string keyCloak = configuration.GetConnectionString("KeyCloak")!;
            services.AddHttpClient("AuthService", configureClient =>
            {
                configureClient.BaseAddress = new Uri(keyCloak);
            });

            services.AddSingleton<IAuthService, AuthService>();

            //var authService = services.BuildServiceProvider().GetRequiredService<IAuthService>();
            //RefitSettings? settings = new()
            //{
            //    AuthorizationHeaderValueGetter = async (sd, cancellationToken) =>
            //    {
            //        using var scope = services.BuildServiceProvider().CreateScope();
            //        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            //        return await authService.GetTokenAsync(cancellationToken);
            //    }
            //};

            services.AddTransient<AuthHandler>();

            services
                .AddRefitClient<IEntriesApi>()
                .AddHttpMessageHandler<AuthHandler>()
                .ConfigureHttpClient(x =>
                    {

                        x.BaseAddress = new Uri(configuration.GetConnectionString("EntriesApi")!);
                    }
                    );

            services.AddTransient<IPartialDailyGateway, PartialDailyGateway>();
            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<ICacheGateway, RedisCacheService>();
            return services;
        }
    }
}
