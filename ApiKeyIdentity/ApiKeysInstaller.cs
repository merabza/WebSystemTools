using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace ApiKeyIdentity;

// ReSharper disable once ClassNeverInstantiated.Global

// ReSharper disable once UnusedType.Global
public sealed class ApiKeysInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

        builder.Services
            .AddAuthentication(x => x.DefaultAuthenticateScheme = AuthenticationSchemaNames.ApiKeyAuthentication)
            .AddApiKeyAuthenticationSchema();

        builder.Services.AddAuthorization();

        builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();


        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Finished");

        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Started");

        app.UseAuthorization();

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Finished");

        return true;
    }
}