using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class MediatorDependencyInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration configuration,
        bool debugMode, params Assembly[] assemblies)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddMediator)} Started");

        var mediatRSettings = configuration.GetSection("MediatRLicenseKey");

        var mediatRLicenseKey = mediatRSettings.Value;

        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = mediatRLicenseKey;

            foreach (var assembly in assemblies) cfg.RegisterServicesFromAssembly(assembly);
        });

        if (debugMode)
            Console.WriteLine($"{nameof(AddMediator)} Finished ");

        return services;
    }
}