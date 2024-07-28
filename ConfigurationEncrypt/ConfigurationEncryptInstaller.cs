using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SystemToolsShared;
using WebInstallers;

namespace ConfigurationEncrypt;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ConfigurationEncryptInstaller : IInstaller
{
    public const string AppKeyKey = nameof(AppKeyKey);
    public int InstallPriority => 10;
    public int ServiceUsePriority => 10;

    public void InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

        var appKey = string.Empty;
        if (parameters.TryGetValue(AppKeyKey, out var parameter))
            appKey = parameter;

        var key = appKey + Environment.MachineName.Capitalize();

        var pathToContentRoot = Directory.GetCurrentDirectory();


        if (!Debugger.IsAttached && SystemStat.IsWindows())
        {
            var newPathToContentRoot = StShared.GetMainModulePath();
            if (newPathToContentRoot is not null)
                pathToContentRoot = newPathToContentRoot;

            Console.WriteLine("!Debugger.IsAttached && IsWindows() so pathToContentRoot=" + pathToContentRoot);
        }


        var config = new ConfigurationBuilder().SetBasePath(pathToContentRoot)
            .AddEncryptedJsonFile(Path.Combine(pathToContentRoot, "appsettingsEncoded.json"), false, true, key,
                Path.Combine(pathToContentRoot, "appsetenkeys.json")).Build();
        builder.Configuration.AddConfiguration(config);

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Finished");
    }

    public void UseServices(WebApplication app, bool debugMode)
    {

    }
}