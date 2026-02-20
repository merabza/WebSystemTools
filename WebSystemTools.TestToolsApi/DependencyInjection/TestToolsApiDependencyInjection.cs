using Microsoft.AspNetCore.Routing;
using Serilog;
using WebSystemTools.TestToolsApi.Endpoints.V1;

namespace WebSystemTools.TestToolsApi.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class TestToolsApiDependencyInjection
{
    public static bool UseTestToolsApiEndpoints(this IEndpointRouteBuilder endpoints, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseTestToolsApiEndpoints));

        endpoints.UseTestEndpoints(debugLogger);

        debugLogger?.Information("{MethodName} Finished", nameof(UseTestToolsApiEndpoints));

        return true;
    }
}
