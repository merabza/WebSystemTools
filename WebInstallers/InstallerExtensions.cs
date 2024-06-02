using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebInstallers;

public static class InstallerExtensions
{
    //public static void InstallServices(this WebApplicationBuilder builder, string[] args,
    //    Dictionary<string, string> parameters, params Type[] scanMarkers)
    //{
    //    InstallServices(builder, args, parameters, scanMarkers.Select(x => x.Assembly).ToArray());
    //}

    public static void InstallServices(this WebApplicationBuilder builder, string[] args,
        Dictionary<string, string> parameters, params Assembly[] assemblies)
    {
        var installers = new List<IInstaller>();

        foreach (var assembly in assemblies)
            installers.AddRange(assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
                .Select(Activator.CreateInstance).Cast<IInstaller>());

        foreach (var installer in installers.OrderBy(x => x.InstallPriority).Distinct())
            installer.InstallServices(builder, args, parameters);

        builder.Services.AddSingleton(installers as IReadOnlyCollection<IInstaller>);
    }

    public static void UseServices(this WebApplication app)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IInstaller>>();

        foreach (var endpointDefinition in definitions.OrderBy(x => x.ServiceUsePriority).Distinct())
            endpointDefinition.UseServices(app);
    }
}