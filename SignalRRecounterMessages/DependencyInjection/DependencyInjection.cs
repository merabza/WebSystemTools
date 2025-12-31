using System;
using Microsoft.AspNetCore.Routing;
using SignalRRecounterMessages.Endpoints.V1;

namespace SignalRRecounterMessages.DependencyInjection;

public static class DependencyInjection
{
    public static bool UseSignalRRecounterMessages(this IEndpointRouteBuilder endpoints, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseSignalRRecounterMessages)} Started");

        endpoints.MapReCounterMessagesEndpoints(debugMode);

        if (debugMode)
            Console.WriteLine($"{nameof(UseSignalRRecounterMessages)} Finished");

        return true;
    }
}