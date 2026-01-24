using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SystemTools.ReCounterAbstraction;

namespace SignalRRecounterMessages.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class SignalRRecounterMessagesDependencyInjection
{
    public static IServiceCollection AddSignalRRecounterMessages(this IServiceCollection services, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddSignalRRecounterMessages));

        services.AddSingleton<IProgressDataManager, ProgressDataManager>();

        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<ReCounterMessagesHub>(options => { options.EnableDetailedErrors = true; });

        debugLogger?.Information("{MethodName} Finished", nameof(AddSignalRRecounterMessages));

        return services;
    }
}