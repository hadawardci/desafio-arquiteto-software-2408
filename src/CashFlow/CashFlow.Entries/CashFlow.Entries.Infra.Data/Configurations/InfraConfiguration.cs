using CashFlow.Entries.Infra.Data.Contracts;
using CashFlow.Entries.Infra.Data.Gateways;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace CashFlow.Entries.Infra.Data.DatabaseConfigurations
{
    public static class InfraConfiguration
    {
        public static IServiceCollection ConfigureInfra(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection("Database").Get<DatabaseConfigurationValue>()!);

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

            services.AddSingleton(sp =>
            {
                var factory = new ConnectionFactory() { HostName = configuration.GetConnectionString("Rabbitmq") };
                return factory.CreateConnection();
            });

            services.AddSingleton<IDatabaseContext, DatabaseContext>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMessagePublisherGateway, RabbitMqProducerService>();
            services.AddScoped<ICacheGateway, RedisCacheService>();
            return services;
        }
    }
}
