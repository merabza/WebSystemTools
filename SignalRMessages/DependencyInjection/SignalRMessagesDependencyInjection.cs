using System;
using Microsoft.Extensions.DependencyInjection;
using SystemToolsShared;

namespace SignalRMessages.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class SignalRMessagesDependencyInjection
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