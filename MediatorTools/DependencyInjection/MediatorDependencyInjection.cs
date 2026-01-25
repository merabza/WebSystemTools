using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MediatorTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class MediatorDependencyInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection services, ILogger? debugLogger,
        IConfiguration configuration, params Assembly[] assemblies)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddMediator));

        var mediatRSettings = configuration.GetSection("MediatRLicenseKey");

        var mediatRLicenseKey = mediatRSettings.Value;

        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = mediatRLicenseKey;

            foreach (var assembly in assemblies) cfg.RegisterServicesFromAssembly(assembly);
        });

        debugLogger?.Information("{MethodName} Finished", nameof(AddMediator));

        return services;
    }
}