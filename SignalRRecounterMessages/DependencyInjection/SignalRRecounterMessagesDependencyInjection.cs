using System;
using Microsoft.Extensions.DependencyInjection;
using ReCounterAbstraction;

namespace SignalRRecounterMessages.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class SignalRRecounterMessagesDependencyInjection
{
    public static IServiceCollection AddSignalRRecounterMessages(this IServiceCollection services, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddSignalRRecounterMessages)} Started");

        services.AddSingleton<IProgressDataManager, ProgressDataManager>();

        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<ReCounterMessagesHub>(options => { options.EnableDetailedErrors = true; });

        if (debugMode)
            Console.WriteLine($"{nameof(AddSignalRRecounterMessages)} Finished");

        return services;
    }
}