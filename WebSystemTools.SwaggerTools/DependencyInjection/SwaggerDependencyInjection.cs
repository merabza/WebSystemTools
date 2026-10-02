using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using SystemTools.ApiContracts;
using SystemTools.SystemToolsShared;

namespace WebSystemTools.SwaggerTools.DependencyInjection;

// ReSharper disable once ClassNeverInstantiated.Global
public static class SwaggerDependencyInjection
{
    //Swagger-ში გამოცხადებული API გასაღების სქემის სახელი
    private const string ApiKeySecuritySchemeName = "ApiKey";

    //ძველი ხელმოწერა: true — JWT bearer, false — სქემის გარეშე.
    //API გასაღებით მომუშავე აპლიკაციები ESwaggerSecurityScheme.ApiKey-ს იყენებენ
    public static IServiceCollection AddSwagger(this IServiceCollection services, ILogger? debugLogger,
        bool useSwaggerWithJwtBearer, int versionCount = 1, string? applicationName = null)
    {
        return services.AddSwagger(debugLogger,
            useSwaggerWithJwtBearer ? ESwaggerSecurityScheme.JwtBearer : ESwaggerSecurityScheme.None, versionCount,
            applicationName);
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, ILogger? debugLogger,
        ESwaggerSecurityScheme securityScheme, int versionCount = 1, string? applicationName = null)
    {
        if (debugLogger is not null)
        {
            debugLogger.Information("{MethodName} Started", nameof(AddSwagger));
        }
        else
        {
            return services;
        }

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        string? appName = applicationName ?? StShared.GetMainModuleFileName();

        services.AddSwaggerGen(x =>
        {
            for (int version = 1; version <= versionCount; version++)
            {
                string appVersion = $"v{version}";
                x.SwaggerDoc(appVersion, new OpenApiInfo { Title = $"{appName} API", Version = appVersion });
            }

            switch (securityScheme)
            {
                case ESwaggerSecurityScheme.JwtBearer:
                    AddSecurityScheme(x, JwtBearerDefaults.AuthenticationScheme,
                        new OpenApiSecurityScheme
                        {
                            Description = "JWT Authorization header using the bearer scheme",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey
                        });
                    break;
                case ESwaggerSecurityScheme.ApiKey:
                    //გასაღები query პარამეტრად იგზავნება (?ApiKey=...), როგორც მას WebSystemTools.ApiKeyIdentity კითხულობს
                    AddSecurityScheme(x, ApiKeySecuritySchemeName,
                        new OpenApiSecurityScheme
                        {
                            Description = "API key in the ApiKey query parameter",
                            Name = ApiKeysConstants.ApiKeyParameterName,
                            In = ParameterLocation.Query,
                            Type = SecuritySchemeType.ApiKey
                        });
                    break;
                case ESwaggerSecurityScheme.None:
                    break;
            }
        });

        debugLogger.Information("{MethodName} Finished", nameof(AddSwagger));

        return services;
    }

    //სქემის გამოცხადება და მისი მოთხოვნა ყველა მეთოდისთვის, რომ Swagger UI-ის Authorize-ში შეყვანილი მნიშვნელობა
    //ყოველ მოთხოვნას დაემატოს. მითითებას დოკუმენტი სჭირდება: მის გარეშე მოთხოვნა JSON-ში ცარიელი იწერება
    private static void AddSecurityScheme(SwaggerGenOptions options, string schemeName,
        OpenApiSecurityScheme securityScheme)
    {
        options.AddSecurityDefinition(schemeName, securityScheme);
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemeName, document)] = []
        });
    }

    public static bool UseSwaggerServices(this IApplicationBuilder app, ILogger? debugLogger, int versionCount = 1)
    {
        if (debugLogger is not null)
        {
            debugLogger.Information("{MethodName} Started", nameof(UseSwaggerServices));
        }
        else
        {
            return true;
        }

        app.UseSwagger();

        app.UseSwaggerUI(config =>
        {
            for (int version = 1; version <= versionCount; version++)
            {
                string appVersion = $"v{version}";
                config.SwaggerEndpoint($"/swagger/{appVersion}/swagger.json", appVersion);
            }
        });

        debugLogger.Information("{MethodName} Finished", nameof(UseSwaggerServices));

        return true;
    }
}
