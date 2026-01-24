using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ValidationTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class ValidationDependencyInjection
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, ILogger? debugLogger,
        params Assembly[] assemblies)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddFluentValidation));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        foreach (var assembly in assemblies)
            services.AddValidatorsFromAssembly(assembly);

        debugLogger?.Information("{MethodName} Finished", nameof(AddFluentValidation));

        return services;
    }
}