using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidationInstaller;

// ReSharper disable once UnusedType.Global
public static class ValidationInstaller
{
    public static void InstallValidation(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        foreach (var assembly in assemblies)
            services.AddValidatorsFromAssembly(assembly);
    }
}