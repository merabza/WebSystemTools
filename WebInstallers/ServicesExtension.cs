using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WebInstallers;

public static class ServicesExtension
{
    public static void AddScopedAllServices(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes.Where(x =>
                     typeof(IScopedService).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false }))
            services.AddScoped(type);
    }

    public static void AddScopedServices<T>(this IServiceCollection services)
    {
        var assembly = typeof(T).Assembly;
        foreach (var type in assembly.ExportedTypes.Where(x =>
                     typeof(T).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false }))
            services.AddScoped(type);
    }
}