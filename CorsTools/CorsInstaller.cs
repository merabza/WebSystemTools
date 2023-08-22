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
        //.AllowAnyOrigin() არ მუშაობს sygnalR-სთვის
        //.WithOrigins("http://localhost:3000", "*")//* არ გამოდგება sygnalR-სთვის
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
                policy => policy.WithOrigins("http://localhost:3000", "https://app.grammar.ge",
                        "https://devapp.grammar.ge", "https://dev2app.grammar.ge").AllowAnyHeader()
                    .AllowAnyMethod().AllowCredentials());
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