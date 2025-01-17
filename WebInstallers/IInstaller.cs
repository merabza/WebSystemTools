using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

namespace WebInstallers;

public interface IInstaller
{
    int InstallPriority { get; }
    int ServiceUsePriority { get; }

    bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters);

    bool UseServices(WebApplication app, bool debugMode);
}