using System;
using Microsoft.AspNetCore.Builder;

namespace StaticFilesTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class StaticFilesDependencyInjection
{
    public static bool UseDefaultAndStaticFiles(this IApplicationBuilder app, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseDefaultAndStaticFiles)} Started");

        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (debugMode)
            Console.WriteLine($"{nameof(UseDefaultAndStaticFiles)} Finished");

        return true;
    }
}