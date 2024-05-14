using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SystemToolsShared;
using WebInstallers;

namespace SwaggerTools;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SwaggerInstaller : IInstaller
{
    public int InstallPriority => 25;
    public int ServiceUsePriority => 0;
    public const string AppNameKey = nameof(AppNameKey);
    public const string VersionCountKey = nameof(VersionCountKey);
    public const string UseSwaggerWithJwtBearerKey = nameof(UseSwaggerWithJwtBearerKey);

    private Dictionary<string, string>? _parameters;

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
        _parameters = parameters;
        //Console.WriteLine("SwaggerInstaller.InstallServices Started");
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        var appName = StShared.GetMainModuleFileName();
        if (parameters.TryGetValue(AppNameKey, out var parameter))
            appName = parameter;

        var versionCount = GetVersionCount(parameters);

        var useSwaggerWithJwtBearer = parameters.ContainsKey(UseSwaggerWithJwtBearerKey);

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
                    System.Array.Empty<string>()
                }
            });
        });
        //Console.WriteLine("SwaggerInstaller.InstallServices Finished");
    }

    private static int GetVersionCount(Dictionary<string, string>? parameters)
    {
        var versionCount = 1;
        if (parameters is null || !parameters.TryGetValue(VersionCountKey, out var parameter)) 
            return versionCount;

        if (int.TryParse(parameter, out var vc))
            versionCount = vc;

        return versionCount;
    }

    public void UseServices(WebApplication app)
    {
        //Console.WriteLine("SwaggerInstaller.UseServices Started");

        if (!app.Environment.IsDevelopment())
            return;
        app.UseSwagger();

        var versionCount = GetVersionCount(_parameters);

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