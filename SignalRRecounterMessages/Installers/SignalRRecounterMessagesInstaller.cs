using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ReCounterDom;
using WebInstallers;

namespace SignalRRecounterMessages.Installers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SignalRRecounterMessagesInstaller : IInstaller
{

    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

        builder.Services.AddSingleton<IProgressDataManager, ProgressDataManager>();

        builder.Services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<ReCounterMessagesHub>(options => { options.EnableDetailedErrors = true; });

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Finished");

        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Started");

        //app.UseAuthorization();

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Finished");

        return true;
    }
}