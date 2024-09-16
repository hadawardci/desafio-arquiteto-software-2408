using CashFlow.Entries.Api.Authorization;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

namespace CashFlow.Entries.Api.Configurations
{
    public static class SecurityConfiguration
    {
        private const string _realm_access = "realm_access.roles";

        public static IServiceCollection AddSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddKeycloakWebApiAuthentication(configuration);
            services
                .AddKeycloakAuthorization(configuration)
                .AddAuthorization(options =>
                {
                    //EntriesPolicies(options);

                    EntriesPoliciesRealm(options);
                })
                ;
            return services;
        }

        private static void EntriesPoliciesRealm(Microsoft.AspNetCore.Authorization.AuthorizationOptions options)
        {
            options.AddPolicy(Policies.Entries,
               builder => builder.RequireRealmRoles(
                   Roles.AddEntry,
                   Roles.ViewEntries));
            options.AddPolicy(Policies.PartialDaily,
                builder => builder.RequireRealmRoles(
                    Roles.ViewPartialDaily));
        }

    }
}
