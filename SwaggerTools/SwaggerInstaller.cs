using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SystemToolsShared;
using WebInstallers;

namespace SwaggerTools;

// ReSharper disable once UnusedType.Global
public sealed class SwaggerInstaller : IInstaller
{
    public int InstallPriority => 25;
    public int ServiceUsePriority => 0;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        //Console.WriteLine("SwaggerInstaller.InstallServices Started");
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        var appName = ProgramAttributes.Instance.GetAttribute<string>("AppName");
        //var appVersion = ProgramAttributes.Instance.GetAttribute<string>("AppVersion");
        var versionCount = ProgramAttributes.Instance.GetAttribute<int>("VersionCount");
        var useSwaggerWithJwtBearer = ProgramAttributes.Instance.GetAttribute<bool>("UseSwaggerWithJWTBearer");

        builder.Services.AddSwaggerGen(x =>
        {
            for (var version = 1; version <= versionCount; version++)
            {
                var appVersion = $"v{version}";
                x.SwaggerDoc(appVersion, new OpenApiInfo { Title = $"{appName} API", Version = appVersion });
            }

            if (!useSwaggerWithJwtBearer)
                return;

            x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
                    },
                    new string[] { }
                }
            });
        });
        //Console.WriteLine("SwaggerInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
        //Console.WriteLine("SwaggerInstaller.UseServices Started");

        if (!app.Environment.IsDevelopment())
            return;
        app.UseSwagger();

        var versionCount = ProgramAttributes.Instance.GetAttribute<int>("VersionCount");

        app.UseSwaggerUI(config =>
        {
            for (var version = 1; version <= versionCount; version++)
            {
                var appVersion = $"v{version}";
                config.SwaggerEndpoint($"/swagger/{appVersion}/swagger.json", appVersion);
            }
        });
        //Console.WriteLine("SwaggerInstaller.UseServices Finished");
    }
}