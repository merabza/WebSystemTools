using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ReCounterDom;
using SignalRMessages.Authorization;
using SystemToolsShared;
using WebInstallers;

namespace SignalRMessages.Installers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SignalRMessagesInstaller : IInstaller
{
    public const string SignalRReCounterKey = nameof(SignalRReCounterKey);
    public const string UseApiKeyKey = nameof(UseApiKeyKey);
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

        var useReCounter = parameters.ContainsKey(SignalRReCounterKey);
        var useApiKey = parameters.ContainsKey(UseApiKeyKey);

        if (useReCounter)
            builder.Services.AddSingleton<IProgressDataManager, ProgressDataManager>();

        builder.Services.AddSingleton<IMessagesDataManager, MessagesDataManager>();

        if (useApiKey)
            builder.Services
                .AddAuthentication(x => x.DefaultAuthenticateScheme = AuthenticationSchemaNames.ApiKeyAuthentication)
                .AddApiKeyAuthenticationSchema();
        else
            builder.Services.AddAuthentication();

        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        var signalRServerBuilder = builder.Services.AddSignalR().AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = null;
        }).AddHubOptions<MessagesHub>(options => { options.EnableDetailedErrors = true; });

        if (useReCounter)
            signalRServerBuilder.AddHubOptions<ReCounterMessagesHub>(
                options => { options.EnableDetailedErrors = true; });

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