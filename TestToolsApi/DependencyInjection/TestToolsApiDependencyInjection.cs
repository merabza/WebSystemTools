//Created by CarcassRepositoriesInstallerClassCreator at 8/1/2022 9:35:56 PM

using Microsoft.AspNetCore.Routing;
using System;
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