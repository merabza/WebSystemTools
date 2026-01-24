using Microsoft.AspNetCore.Routing;
using Serilog;
using SignalRRecounterMessages.Endpoints.V1;

namespace SignalRRecounterMessages.DependencyInjection;

public static class DependencyInjection
{
    public static bool UseSignalRRecounterMessages(this IEndpointRouteBuilder endpoints, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseSignalRRecounterMessages));

        endpoints.MapReCounterMessagesEndpoints(debugLogger);

        debugLogger?.Information("{MethodName} Finished", nameof(UseSignalRRecounterMessages));

        return true;
    }
}