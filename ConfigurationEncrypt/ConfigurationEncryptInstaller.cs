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

    public void InstallServices(WebApplicationBuilder builder, string[] args, Dictionary<string, string> parameters)
    {
        //Console.WriteLine("ConfigurationEncryptInstaller.InstallServices Started");

        var appKey = string.Empty;
        if (parameters.TryGetValue(AppKeyKey, out var parameter))
            appKey = parameter;

        var key = appKey + Environment.MachineName.Capitalize();

        var pathToContentRoot = Directory.GetCurrentDirectory();


        if (!Debugger.IsAttached && SystemStat.IsWindows())
        {
            var newPathToContentRoot = StShared.GetMainModulePath();
            if (newPathToContentRoot != null)
                pathToContentRoot = newPathToContentRoot;

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