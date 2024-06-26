//Created by CarcassRepositoriesInstallerClassCreator at 8/1/2022 9:35:56 PM

using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReCounterDom;
using WebInstallers;

namespace ReCounterServiceInstaller.Installers;

// ReSharper disable once UnusedType.Global
public sealed class ReCounterDomInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
        //Console.WriteLine("GeoModelPartInstaller.InstallServices Started");

        builder.Services.AddSingleton<ReCounterQueuedHostedService>();
        builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<ReCounterQueuedHostedService>());
        builder.Services.AddSingleton<IReCounterBackgroundTaskQueue, ReCounterBackgroundTaskQueue>();

        //Console.WriteLine("GeoModelPartInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
    }
}