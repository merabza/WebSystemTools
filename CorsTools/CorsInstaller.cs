//Created by ReactInstallerClassCreator at 8/1/2022 8:38:26 PM

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace CorsTools;

// ReSharper disable once UnusedType.Global
public sealed class CorsInstaller : IInstaller
{
    public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    public int InstallPriority => 15;
    public int ServiceUsePriority => 35;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        //Console.WriteLine("CorsInstaller.InstallServices Started");

        //builder.Services.AddCors();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
                policy => policy.AllowAnyOrigin() /*.WithOrigins("http://localhost:5022", "*")*/.AllowAnyHeader()
                    .AllowAnyMethod());
        });

        //Console.WriteLine("CorsInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
        Console.WriteLine("CorsInstaller.UseMiddleware Started");

        // global cors policy
        //.AllowAnyOrigin()
        //app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(_ => true).AllowCredentials());
        //app.UseCors();
        app.UseCors(MyAllowSpecificOrigins);

        Console.WriteLine("CorsInstaller.UseMiddleware Finished");
    }
}