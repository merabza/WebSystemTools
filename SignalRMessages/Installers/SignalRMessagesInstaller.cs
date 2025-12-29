using System;
using Microsoft.Extensions.DependencyInjection;
using SystemToolsShared;

//using WebInstallers;

namespace SignalRMessages.Installers;

// ReSharper disable once UnusedType.Global
public static class SignalRMessagesInstaller // : IInstaller
{
    //public int InstallPriority => 30;
    //public int ServiceUsePriority => 30;

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

    //public bool UseServices(WebApplication app, bool debugMode)
    //{
    //    if (debugMode)
    //        Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Started");

    //    //app.UseAuthorization();

    //    if (debugMode)
    //        Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Finished");

    //    return true;
    //}
}