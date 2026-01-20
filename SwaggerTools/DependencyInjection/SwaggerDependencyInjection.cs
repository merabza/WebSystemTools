using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using SystemTools.SystemToolsShared;

namespace SwaggerTools.DependencyInjection;

// ReSharper disable once ClassNeverInstantiated.Global
public static class SwaggerDependencyInjection
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, bool debugMode,
        bool useSwaggerWithJwtBearer, int versionCount = 1, string? applicationName = null)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddSwagger)} Started");

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        var appName = applicationName ?? StShared.GetMainModuleFileName();

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
        });

        if (debugMode)
            Console.WriteLine($"{nameof(AddSwagger)} Finished");

        return services;
    }

    public static bool UseSwaggerServices(this IApplicationBuilder app, bool debugMode, int versionCount = 1)
    {
        switch (debugMode)
        {
            case true:
                Console.WriteLine($"{nameof(UseSwaggerServices)} Started");
                break;
            case false:
                return true;
        }

        app.UseSwagger();

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
}