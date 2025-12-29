using System;
using ApiKeysManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

//using WebInstallers;

namespace ApiKeyIdentity.Installers;

// ReSharper disable once ClassNeverInstantiated.Global

// ReSharper disable once UnusedType.Global
public static class ApiKeysInstaller // : IInstaller
{
    //public int InstallPriority => 30;
    //public int ServiceUsePriority => 30;

    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddApiKeyAuthentication)} Started");

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUserByApiKey, CurrentUserByApiKey>();

        services.AddScoped<IApiKeyFinder, ApiKeyByConfigFinder>();

        services.AddAuthentication(x => x.DefaultAuthenticateScheme = AuthenticationSchemaNames.ApiKeyAuthentication)
            .AddApiKeyAuthenticationSchema();

        services.AddAuthorization();

        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

        if (debugMode)
            Console.WriteLine($"{nameof(AddApiKeyAuthentication)} Finished");

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