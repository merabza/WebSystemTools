using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Serilog;
using SystemTools.StringMessagesApiContracts.V1.Routes;

namespace SignalRMessages.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public static class MessagesEndpoints
{
    public static bool UseSignalRMessagesHub(this IEndpointRouteBuilder endpoints, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseSignalRMessagesHub));

        endpoints.MapHub<MessagesHub>(MessagesRoutes.ApiBase + MessagesRoutes.Messages.MessagesRoute,
            options => { options.Transports = HttpTransportType.LongPolling; }).RequireAuthorization();

        debugLogger?.Information("{MethodName} Finished", nameof(UseSignalRMessagesHub));

        return true;
    }
}