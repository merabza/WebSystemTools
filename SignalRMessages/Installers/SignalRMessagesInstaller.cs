using System;
using Microsoft.Extensions.DependencyInjection;
using SystemToolsShared;

namespace SignalRMessages.Installers;

// ReSharper disable once UnusedType.Global
public static class SignalRMessagesInstaller
{
    public static IServiceCollection AddSignalRMessages(this IServiceCollection services, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddSignalRMessages)} Started");

        services.AddSingleton<IMessagesDataManager, MessagesDataManager>();
        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<MessagesHub>(options => { options.EnableDetailedErrors = true; });

        if (debugMode)
            Console.WriteLine($"{nameof(AddSignalRMessages)} Finished");

        return services;
    }
}