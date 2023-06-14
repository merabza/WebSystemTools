using Microsoft.AspNetCore.Builder;

namespace WebInstallers;

public interface IInstaller
{
    int InstallPriority { get; }
    int ServiceUsePriority { get; }


    void InstallServices(WebApplicationBuilder builder, string[] args);
    void UseServices(WebApplication app);
}