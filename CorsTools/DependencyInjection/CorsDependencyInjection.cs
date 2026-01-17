using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CorsTools.DependencyInjection;

// ReSharper disable once ClassNeverInstantiated.Global
public static class CorsDependencyInjection
{
    public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static IServiceCollection AddCorsService(this IServiceCollection services, ILogger logger,
        IConfiguration configuration, bool debugMode)
    {
        if (debugMode)
            logger.Information("{MethodName} Started", nameof(AddCorsService));

        var corsSettings = configuration.GetSection("CorsSettings");

        var originsSection = corsSettings.GetChildren().SingleOrDefault(s => s.Key == "Origins");

        if (originsSection is null)
            return services;

        //.WithOrigins("*")//* არ გამოდგება sygnalR-სთვის
        services.AddCors(options =>
        {
            var origins = (from child in originsSection.GetChildren() where child.Value is not null select child.Value)
                .ToArray();
            options.AddPolicy(MyAllowSpecificOrigins,
                policy => policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
        });

        if (debugMode)
            logger.Information("{MethodName} Finished", nameof(AddCorsService));

        return services;
    }

    public static bool UseCorsService(this IApplicationBuilder app, ILogger logger, bool debugMode)
    {
        if (debugMode)
            logger.Information("{MethodName} Started", nameof(UseCorsService));

        app.UseCors(MyAllowSpecificOrigins);

        if (debugMode)
            logger.Information("{MethodName} Finished", nameof(UseCorsService));

        return true;
    }
}