using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using SystemTools.ApiKeysManagement;

namespace ApiKeyIdentity.DependencyInjection;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedType.Global
public static class ApiKeyIdentityDependencyInjection
{
    public static IServiceCollection AddApiKeyIdentity(this IServiceCollection services, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddApiKeyIdentity));

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUserByApiKey, CurrentUserByApiKey>();

        services.AddScoped<IApiKeyFinder, ApiKeyByConfigFinder>();

        services.AddAuthentication(x => x.DefaultAuthenticateScheme = AuthenticationSchemaNames.ApiKeyAuthentication)
            .AddApiKeyAuthenticationSchema();

        services.AddAuthorization();

        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

        debugLogger?.Information("{MethodName} Finished", nameof(AddApiKeyIdentity));

        return services;
    }

    public static bool UseApiKeysAuthorization(this IApplicationBuilder app, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseApiKeysAuthorization));

        app.UseAuthorization();

        debugLogger?.Information("{MethodName} Finished", nameof(UseApiKeysAuthorization));

        return true;
    }
}