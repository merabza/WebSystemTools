using System;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ValidationTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class ValidationDependencyInjection
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, bool debugMode,
        params Assembly[] assemblies)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddFluentValidation)} Started");

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        foreach (var assembly in assemblies)
            services.AddValidatorsFromAssembly(assembly);

        if (debugMode)
            Console.WriteLine($"{nameof(AddFluentValidation)} Finished ");

        return services;
    }
}