using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using SignalRContracts.V1.Routes;
using WebInstallers;

namespace SignalRMessages.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public sealed class MessagesEndpoints : IInstaller
{
    public int InstallPriority => 70;

    public int ServiceUsePriority => 70;

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
    }

    public void UseServices(WebApplication app)
    {
        //Console.WriteLine("WebAgentMessagesEndpoints.UseServices Started");

        app.MapHub<MessagesHub>(MessagesRoutes.Messages.MessagesRoute,
            options => { options.Transports = HttpTransportType.LongPolling; }).RequireAuthorization();

        //Console.WriteLine("WebAgentMessagesEndpoints.UseServices Finished");
    }
}