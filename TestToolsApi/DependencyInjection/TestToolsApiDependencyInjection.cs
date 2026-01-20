using System;
using Microsoft.AspNetCore.Routing;
using TestToolsApi.Endpoints.V1;

namespace TestToolsApi.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class TestToolsApiDependencyInjection
{
    public static bool UseTestToolsApiEndpoints(this IEndpointRouteBuilder endpoints, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseTestToolsApiEndpoints)} Started");

        endpoints.UseTestEndpoints(debugMode);

        if (debugMode)
            Console.WriteLine($"{nameof(UseTestToolsApiEndpoints)} Finished");

        return true;
    }
}