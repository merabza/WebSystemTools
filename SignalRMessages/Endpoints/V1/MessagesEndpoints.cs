using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using StringMessagesApiContracts.V1.Routes;
using System;
//using WebInstallers;

namespace SignalRMessages.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public static class MessagesEndpoints// : IInstaller
{
    //public int InstallPriority => 70;

    //public int ServiceUsePriority => 70;

    //public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
    //    Dictionary<string, string> parameters)
    //{
    //    return true;
    //}

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