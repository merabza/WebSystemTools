using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ReCounterDom;
using SignalRMessages.Authorization;
using SystemToolsShared;
using WebInstallers;

namespace SignalRMessages.Installers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SignalRMessagesInstaller : IInstaller
{
    public const string SignalRReCounterKey = nameof(SignalRReCounterKey);
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
        //Console.WriteLine("WebAgentMessagesInstaller.InstallServices Started");
        var useReCounter = parameters.ContainsKey(SignalRReCounterKey);

        if (useReCounter)
            builder.Services.AddSingleton<IProgressDataManager, ProgressDataManager>();

        builder.Services.AddSingleton<IMessagesDataManager, MessagesDataManager>();

        builder.Services.AddAuthentication(x =>
                x.DefaultAuthenticateScheme = AuthenticationSchemaNames.ApiKeyAuthentication)
            .AddApiKeyAuthenticationSchema();
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        var signalRServerBuilder = builder.Services.AddSignalR().AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = null;
        }).AddHubOptions<MessagesHub>(options => { options.EnableDetailedErrors = true; });

        if (useReCounter)
            signalRServerBuilder.AddHubOptions<ReCounterMessagesHub>(
                options => { options.EnableDetailedErrors = true; });
        //Console.WriteLine("WebAgentMessagesInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
        app.UseAuthorization();
    }
}