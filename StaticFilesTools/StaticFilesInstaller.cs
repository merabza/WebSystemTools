//Created by StaticFilesInstallerClassCreator at 8/1/2022 8:38:26 PM

using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using WebInstallers;

namespace StaticFilesTools;

// ReSharper disable once UnusedType.Global
public sealed class StaticFilesInstaller : IInstaller
{
    public int InstallPriority => 15;
    public int ServiceUsePriority => 135;

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
        //Console.WriteLine("StaticFilesInstaller.InstallServices Started");

        //// In production, the React files will be served from this directory
        //builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

        //Console.WriteLine("StaticFilesInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
        //Console.WriteLine("StaticFilesInstaller.UseMiddleware Started");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

        app.UseDefaultFiles();
        app.UseStaticFiles();
        //app.UseSpaStaticFiles();

        //app.UseSpa(spa =>
        //{
        //    spa.Options.SourcePath = "ClientApp";
        //    if (app.Environment.IsDevelopment()) spa.UseReactDevelopmentServer("start");
        //});

        //Console.WriteLine("StaticFilesInstaller.UseMiddleware Finished");
    }
}