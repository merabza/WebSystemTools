using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using SystemToolsShared;

//using WebInstallers;

namespace SwaggerTools;

// ReSharper disable once ClassNeverInstantiated.Global
public static class SwaggerInstaller // : IInstaller
{
    //public const string AppNameKey = nameof(AppNameKey);
    //public const string VersionCountKey = nameof(VersionCountKey);
    //public const string UseSwaggerWithJwtBearerKey = nameof(UseSwaggerWithJwtBearerKey);

    //private Dictionary<string, string>? _parameters;
    //public int InstallPriority => 25;
    //public int ServiceUsePriority => 0;

    public static IServiceCollection AddSwagger(this IServiceCollection services, bool debugMode,
        bool useSwaggerWithJwtBearer, int versionCount = 1, string? applicationName = null)
    {
        //_parameters = parameters;

        if (debugMode)
            Console.WriteLine($"{nameof(AddSwagger)} Started");

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        var appName = applicationName ?? StShared.GetMainModuleFileName();

        //var versionCount = GetVersionCount(parameters);

        //var useSwaggerWithJwtBearer = parameters.ContainsKey(UseSwaggerWithJwtBearerKey);

        services.AddSwaggerGen(x =>
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

            var oas = new OpenApiSecurityRequirement();
            var b = new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme);

            oas.Add(b, [nameof(ReferenceType.SecurityScheme)]);

            x.AddSecurityRequirement(_ => oas);

            //x.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference
            //            {
            //                Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme
            //            }
            //        },
            //        Array.Empty<string>()
            //    }
            //});
        });

        if (debugMode)
            Console.WriteLine($"{nameof(AddSwagger)} Finished");

        return services;
    }

    public static bool UseSwaggerServices(this WebApplication app, bool debugMode, int versionCount = 1)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseSwaggerServices)} Started");

        if (!app.Environment.IsDevelopment())
            return true;
        app.UseSwagger();

        //var versionCount = GetVersionCount(_parameters);

        app.UseSwaggerUI(config =>
        {
            for (var version = 1; version <= versionCount; version++)
            {
                var appVersion = $"v{version}";
                config.SwaggerEndpoint($"/swagger/{appVersion}/swagger.json", appVersion);
            }
        });

        if (debugMode)
            Console.WriteLine($"{nameof(UseSwaggerServices)} Finished");

        return true;
    }

    //private static int GetVersionCount(Dictionary<string, string>? parameters)
    //{
    //    var versionCount = 1;
    //    if (parameters is null || !parameters.TryGetValue(VersionCountKey, out var parameter))
    //        return versionCount;

    //    if (int.TryParse(parameter, out var vc))
    //        versionCount = vc;

    //    return versionCount;
    //}
}