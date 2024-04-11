using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SystemToolsShared;
using WebInstallers;

namespace ConfigurationEncrypt;

// ReSharper disable once UnusedType.Global
public sealed class ConfigurationEncryptInstaller : IInstaller
{
    public int InstallPriority => 10;
    public int ServiceUsePriority => 10;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        //Console.WriteLine("ConfigurationEncryptInstaller.InstallServices Started");


        var key = ProgramAttributes.Instance.GetAttribute<string>("AppKey") + Environment.MachineName.Capitalize();

        var pathToContentRoot = Directory.GetCurrentDirectory();


        if (!Debugger.IsAttached && SystemStat.IsWindows())
        {
            // ReSharper disable once using
            using var processModule = Process.GetCurrentProcess().MainModule;
            var pathToExe = processModule?.FileName;
            if (pathToExe != null)
            {
                var newPathToContentRoot = Path.GetDirectoryName(pathToExe);
                if (newPathToContentRoot != null)
                    pathToContentRoot = newPathToContentRoot;
            }

            Console.WriteLine("!Debugger.IsAttached && IsWindows() so pathToContentRoot=" + pathToContentRoot);
        }


        var config = new ConfigurationBuilder().SetBasePath(pathToContentRoot)
            .AddEncryptedJsonFile(Path.Combine(pathToContentRoot, "appsettingsEncoded.json"), false, true, key,
                Path.Combine(pathToContentRoot, "appsetenkeys.json")).Build();
        builder.Configuration.AddConfiguration(config);

        //Console.WriteLine("ConfigurationEncryptInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
    }
}