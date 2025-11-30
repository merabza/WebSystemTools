using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace WebInstallers;

public static class ServicesExtension
{
    public static void AddScopedAllServices(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.ExportedTypes.Where(x =>
                     typeof(IScopedService).IsAssignableFrom(x) &&
                     x is { IsInterface: false, IsAbstract: false }))
            services.AddScoped(type);
    }
}