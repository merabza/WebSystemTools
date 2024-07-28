using System;
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

    public void InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        //Console.WriteLine("StaticFilesInstaller.InstallServices Started");

        //// In production, the React files will be served from this directory
        //builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

        //Console.WriteLine("StaticFilesInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Started");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(UseServices)} Finished");
    }
}