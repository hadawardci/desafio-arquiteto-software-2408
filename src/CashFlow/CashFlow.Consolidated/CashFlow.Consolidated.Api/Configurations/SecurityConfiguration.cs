using CashFlow.Consolidated.Api.Authorization;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

namespace CashFlow.Consolidated.Api.Configurations
{
    public static class SecurityConfiguration
    {
        public static IServiceCollection AddSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddKeycloakWebApiAuthentication(configuration);
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy(Policies.ConsolidatedDaily,
                        builder => builder.RequireRealmRoles(
                            Roles.ViewConsolidated));
                })
                .AddKeycloakAuthorization(configuration);

            return services;
        }
    }
}
