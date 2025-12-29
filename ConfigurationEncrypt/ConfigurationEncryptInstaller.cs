using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using SystemToolsShared;

//using WebInstallers;

namespace ConfigurationEncrypt;

// ReSharper disable once ClassNeverInstantiated.Global
public static class ConfigurationEncryptInstaller // : IInstaller
{
    //public const string AppKeyKey = nameof(AppKeyKey);
    //public int InstallPriority => 10;
    //public int ServiceUsePriority => 10;

    public static bool AddConfigurationEncryption(this IConfigurationBuilder configurationBuilder, bool debugMode,
        string appKey)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddConfigurationEncryption)} Started");

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
        configurationBuilder.AddConfiguration(config);

        if (debugMode)
            Console.WriteLine($"{nameof(AddConfigurationEncryption)} Finished");

        return true;
    }

    //public bool UseServices(WebApplication app, bool debugMode)
    //{
    //    return true;
    //}
}