//Created by CarcassRepositoriesInstallerClassCreator at 8/1/2022 9:35:56 PM

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReCounterDom;
using WebInstallers;

namespace ReCounterServiceInstaller;

// ReSharper disable once UnusedType.Global
public sealed class ReCounterInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

        builder.Services.AddSingleton<ReCounterQueuedHostedService>();
        builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<ReCounterQueuedHostedService>());
        builder.Services.AddSingleton<IReCounterBackgroundTaskQueue, ReCounterBackgroundTaskQueue>();

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Finished");

        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        return true;
    }
}