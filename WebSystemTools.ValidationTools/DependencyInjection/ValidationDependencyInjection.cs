using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace WebSystemTools.ValidationTools.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class ValidationDependencyInjection
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services, ILogger? debugLogger,
        params Assembly[] assemblies)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddFluentValidation));

        //ძველი MediatR-ის ValidationBehavior დაკომენტარებულია — ვალიდაცია ახლა
        //SystemTools.Application.Abstractions-ის დეკორატორებით ხდება.
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        foreach (Assembly assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }

        debugLogger?.Information("{MethodName} Finished", nameof(AddFluentValidation));

        return services;
    }
}
