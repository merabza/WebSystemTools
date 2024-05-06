using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace RazorPagesTools;

// ReSharper disable once UnusedType.Global
public sealed class RazorPagesInstaller : IInstaller
{
    public int InstallPriority => 14;
    public int ServiceUsePriority => 136;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        //Console.WriteLine("StaticFilesInstaller.InstallServices Started");

        builder.Services.AddRazorPages();

        //Console.WriteLine("StaticFilesInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
        //Console.WriteLine("StaticFilesInstaller.UseMiddleware Started");

        app.UseRouting();

        //app.UseAuthorization();

        app.MapRazorPages();

        //Console.WriteLine("StaticFilesInstaller.UseMiddleware Finished");
    }
}