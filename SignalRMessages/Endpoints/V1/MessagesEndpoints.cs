using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using StringMessagesApiContracts.V1.Routes;
using WebInstallers;

namespace SignalRMessages.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public sealed class MessagesEndpoints : IInstaller
{
    public int InstallPriority => 70;

    public int ServiceUsePriority => 70;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Started");

        app.MapHub<MessagesHub>(MessagesRoutes.ApiBase + MessagesRoutes.Messages.MessagesRoute,
            options => { options.Transports = HttpTransportType.LongPolling; }).RequireAuthorization();


        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Finished");

        return true;
    }
}