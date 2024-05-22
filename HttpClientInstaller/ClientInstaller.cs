using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebInstallers;

namespace HttpClientInstaller;

// ReSharper disable once UnusedType.Global
public sealed class ClientInstaller : IInstaller
{
    public int InstallPriority => 10;
    public int ServiceUsePriority => 10;

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
        //Console.WriteLine("ConfigurationEncryptInstaller.InstallServices Started");

        builder.Services.AddHttpClient();

        //Console.WriteLine("ConfigurationEncryptInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
    }
}