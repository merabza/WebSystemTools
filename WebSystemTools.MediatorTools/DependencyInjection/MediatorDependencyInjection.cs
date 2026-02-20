using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace WebSystemTools.MediatorTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class MediatorDependencyInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection services, ILogger? debugLogger,
        IConfiguration configuration, params Assembly[] assemblies)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddMediator));

        IConfigurationSection mediatRSettings = configuration.GetSection("MediatRLicenseKey");

        string? mediatRLicenseKey = mediatRSettings.Value;

        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = mediatRLicenseKey;

            foreach (Assembly assembly in assemblies)
            {
                cfg.RegisterServicesFromAssembly(assembly);
            }
        });

        debugLogger?.Information("{MethodName} Finished", nameof(AddMediator));

        return services;
    }
}
