using Microsoft.AspNetCore.Builder;
using Serilog;

namespace StaticFilesTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class StaticFilesDependencyInjection
{
    public static bool UseDefaultAndStaticFiles(this IApplicationBuilder app, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseDefaultAndStaticFiles));

        app.UseDefaultFiles();
        app.UseStaticFiles();

        debugLogger?.Information("{MethodName} Finished", nameof(UseDefaultAndStaticFiles));

        return true;
    }
}