using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace AllowSynchronousIoTool;

// ReSharper disable once UnusedType.Global
public class KestrelServerAllowSynchronousIoOptionInstaller : IInstaller
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