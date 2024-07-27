using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace CorsTools;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CorsInstaller : IInstaller
{
    public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    public int InstallPriority => 15;
    public int ServiceUsePriority => 35;

    public void InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine("CorsInstaller.InstallServices Started");
        var corsSettings = builder.Configuration.GetSection("CorsSettings");

        var originsSection = corsSettings.GetChildren().SingleOrDefault(s => s.Key == "Origins");

        if (originsSection is null)
            return;


        //.WithOrigins("*")//* არ გამოდგება sygnalR-სთვის
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
                policy => policy
                    .WithOrigins((from child in originsSection.GetChildren()
                        where child.Value is not null
                        select child.Value).ToArray()).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
        });

        if (debugMode)
            Console.WriteLine("CorsInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine("CorsInstaller.UseMiddleware Started");

        app.UseCors(MyAllowSpecificOrigins);

        if (debugMode)
            Console.WriteLine("CorsInstaller.UseMiddleware Finished");
    }
}