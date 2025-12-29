using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

//using WebInstallers;

namespace StaticFilesTools;

// ReSharper disable once UnusedType.Global
public static class StaticFilesInstaller // : IInstaller
{
    //public int InstallPriority => 15;
    //public int ServiceUsePriority => 135;

    //public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
    //    Dictionary<string, string> parameters)
    //{
    //    //Console.WriteLine("StaticFilesInstaller.InstallServices Started");

    //    //// In production, the React files will be served from this directory
    //    //builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

    //    //Console.WriteLine("StaticFilesInstaller.InstallServices Finished");

    //    return true;
    //}

    public static bool UseDefaultAndStaticFiles(this WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseDefaultAndStaticFiles)} Started");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (debugMode)
            Console.WriteLine($"{nameof(UseDefaultAndStaticFiles)} Finished");

        return true;
    }
}