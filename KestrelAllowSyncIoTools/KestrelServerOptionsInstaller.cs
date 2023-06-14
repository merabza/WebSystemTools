//Created by KestrelServerOptionsInstallerClassCreator at 8/5/2022 7:36:07 AM

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace KestrelAllowSyncIoTools;

// ReSharper disable once UnusedType.Global
public class KestrelServerOptionsInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        Console.WriteLine("KestrelServerOptionsInstaller.InstallServices Started");

        builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

        Console.WriteLine("KestrelServerOptionsInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
    }
}