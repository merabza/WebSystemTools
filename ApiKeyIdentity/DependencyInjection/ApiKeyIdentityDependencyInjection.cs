using System;
using ApiKeysManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ApiKeyIdentity.DependencyInjection;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedType.Global
public static class ApiKeyIdentityDependencyInjection
{
    public static IServiceCollection AddApiKeyIdentity(this IServiceCollection services, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddApiKeyIdentity)} Started");

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUserByApiKey, CurrentUserByApiKey>();

        services.AddScoped<IApiKeyFinder, ApiKeyByConfigFinder>();

        services.AddAuthentication(x => x.DefaultAuthenticateScheme = AuthenticationSchemaNames.ApiKeyAuthentication)
            .AddApiKeyAuthenticationSchema();

        services.AddAuthorization();

        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

        if (debugMode)
            Console.WriteLine($"{nameof(AddApiKeyIdentity)} Finished");

        return services;
    }

    public static bool UseApiKeysAuthorization(this IApplicationBuilder app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseApiKeysAuthorization)} Started");

        app.UseAuthorization();

        if (debugMode)
            Console.WriteLine($"{nameof(UseApiKeysAuthorization)} Finished");

        return true;
    }
}