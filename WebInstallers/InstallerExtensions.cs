using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebInstallers;

public static class InstallerExtensions
{
    public static void InstallServices(this WebApplicationBuilder builder, string[] args, params Type[] scanMarkers)
    {
        InstallServices(builder, args, scanMarkers.Select(x => x.Assembly).ToArray());
    }

    public static void InstallServices(this WebApplicationBuilder builder, string[] args, params Assembly[] assemblies)
    {
        var installers = new List<IInstaller>();

        foreach (var assembly in assemblies)
            installers.AddRange(assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>());

        foreach (var installer in installers.OrderBy(x => x.InstallPriority)) installer.InstallServices(builder, args);

        builder.Services.AddSingleton(installers as IReadOnlyCollection<IInstaller>);
    }

    public static void UseServices(this WebApplication app)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IInstaller>>();

        foreach (var endpointDefinition in definitions.OrderBy(x => x.ServiceUsePriority))
            endpointDefinition.UseServices(app);
    }
}