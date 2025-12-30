using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CorsTools.DependencyInjection;

// ReSharper disable once ClassNeverInstantiated.Global
public static class CorsDependencyInjection
{
    public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static IServiceCollection AddCorsService(this IServiceCollection services, IConfiguration configuration,
        bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddCorsService)} Started");

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
            Console.WriteLine($"{nameof(AddCorsService)} Finished");

        return services;
    }

    public static bool UseCorsService(this IApplicationBuilder app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseCorsService)} Started");

        app.UseCors(MyAllowSpecificOrigins);

        if (debugMode)
            Console.WriteLine($"{nameof(UseCorsService)} Finished");

        return true;
    }
}