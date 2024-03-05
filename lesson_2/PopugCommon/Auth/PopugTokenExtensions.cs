using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationServer
{
    public static class PopugTokenExtensions
    {
        public static IServiceCollection AddAuthClient(
            this IServiceCollection services,
            string optionsSectionName = "PopugAuthClient", Action<IHttpClientBuilder, string> clientBuilder = null) => services;

        public static IServiceCollection AddPopugTokenAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthClient()
                .AddAuthentication(o => o.DefaultScheme = PopugTokenScheme.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, PopugTokenAuthenticationHandler>(PopugTokenScheme.SchemeName, null);

            return services;
        }

    }
}