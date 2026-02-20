using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SystemTools.SystemToolsShared;

namespace WebSystemTools.SignalRMessages.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class SignalRMessagesDependencyInjection
{
    public static IServiceCollection AddSignalRMessages(this IServiceCollection services, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddSignalRMessages));

        services.AddSingleton<IMessagesDataManager, MessagesDataManager>();
        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<MessagesHub>(options => { options.EnableDetailedErrors = true; });

        debugLogger?.Information("{MethodName} Finished", nameof(AddSignalRMessages));

        return services;
    }
}
