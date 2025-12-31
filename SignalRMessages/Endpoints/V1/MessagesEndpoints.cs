using Microsoft.AspNetCore.Http.Connections;
using StringMessagesApiContracts.V1.Routes;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SignalRMessages.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public static class MessagesEndpoints// : IInstaller
{
    public static bool UseSignalRMessagesHub(this IEndpointRouteBuilder endpoints, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseSignalRMessagesHub)} Started");

        endpoints.MapHub<MessagesHub>(MessagesRoutes.ApiBase + MessagesRoutes.Messages.MessagesRoute,
            options => { options.Transports = HttpTransportType.LongPolling; }).RequireAuthorization();

        if (debugMode)
            Console.WriteLine($"{nameof(UseSignalRMessagesHub)} Finished");

        return true;
    }
}