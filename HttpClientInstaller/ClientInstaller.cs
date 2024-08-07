using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace HttpClientInstaller;

// ReSharper disable once UnusedType.Global
public sealed class ClientInstaller : IInstaller
{
    public int InstallPriority => 10;
    public int ServiceUsePriority => 10;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

        builder.Services.AddHttpClient();

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Finished");

        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        return true;
    }
}