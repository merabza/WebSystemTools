using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebInstallers;

public static class InstallerExtensions
{
    public static bool InstallServices(this WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters, params Assembly[] assemblies)
    {
        if (debugMode)
            Console.WriteLine("InstallerExtensions.InstallServices Started");

        var installers = new List<IInstaller>();

        foreach (var assembly in assemblies)
            installers.AddRange(assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
                .Select(Activator.CreateInstance).Cast<IInstaller>());

        if (installers.OrderBy(x => x.InstallPriority).Distinct().Any(installer =>
                !installer.InstallServices(builder, debugMode, args, parameters)))
            return false;

        builder.Services.AddSingleton(installers as IReadOnlyCollection<IInstaller>);

        if (debugMode)
            Console.WriteLine("InstallerExtensions.InstallServices Finished");

        return true;
    }

    public static bool UseServices(this WebApplication app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine("InstallerExtensions.UseServices Started");

        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IInstaller>>();

        if (definitions.OrderBy(x => x.ServiceUsePriority).Distinct()
            .Any(endpointDefinition => !endpointDefinition.UseServices(app, debugMode)))
            return false;

        if (debugMode)
            Console.WriteLine("InstallerExtensions.UseServices Finished");

        return true;
    }
}