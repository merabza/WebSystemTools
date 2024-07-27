using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebInstallers;

public static class InstallerExtensions
{

    public static void InstallServices(this WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters, params Assembly[] assemblies)
    {
        var installers = new List<IInstaller>();

        foreach (var assembly in assemblies)
            installers.AddRange(assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
                .Select(Activator.CreateInstance).Cast<IInstaller>());

        foreach (var installer in installers.OrderBy(x => x.InstallPriority).Distinct())
            installer.InstallServices(builder, debugMode, args, parameters);

        builder.Services.AddSingleton(installers as IReadOnlyCollection<IInstaller>);
    }

    public static void UseServices(this WebApplication app, bool debugMode)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IInstaller>>();

        foreach (var endpointDefinition in definitions.OrderBy(x => x.ServiceUsePriority).Distinct())
            endpointDefinition.UseServices(app, debugMode);
    }
}