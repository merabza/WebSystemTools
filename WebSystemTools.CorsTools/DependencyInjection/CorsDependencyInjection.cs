using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace WebSystemTools.CorsTools.DependencyInjection;

// ReSharper disable once ClassNeverInstantiated.Global
public static class CorsDependencyInjection
{
    public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static IServiceCollection AddCorsService(this IServiceCollection services, ILogger? debugLogger,
        IConfiguration configuration)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddCorsService));

        IConfigurationSection corsSettings = configuration.GetSection("CorsSettings");

        IConfigurationSection? originsSection = corsSettings.GetChildren().SingleOrDefault(s => s.Key == "Origins");

        if (originsSection is null)
        {
            return services;
        }

        //.WithOrigins("*")//* არ გამოდგება sygnalR-სთვის
        services.AddCors(options =>
        {
            string[] origins =
                (from child in originsSection.GetChildren() where child.Value is not null select child.Value).ToArray();
            options.AddPolicy(MyAllowSpecificOrigins,
                policy => policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
        });

        debugLogger?.Information("{MethodName} Finished", nameof(AddCorsService));

        return services;
    }

    public static bool UseCorsService(this IApplicationBuilder app, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseCorsService));

        app.UseCors(MyAllowSpecificOrigins);

        debugLogger?.Information("{MethodName} Finished", nameof(UseCorsService));

        return true;
    }
}
