using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using SystemTools.SystemToolsShared;

namespace ConfigurationEncrypt;

// ReSharper disable once ClassNeverInstantiated.Global
public static class ConfigurationEncryptHostBuilderExtensions
{
    public static bool AddConfigurationEncryption(this IConfigurationBuilder configurationBuilder, ILogger logger,
        bool debugMode, string appKey)
    {
        if (debugMode)
            logger.Information($"{nameof(AddConfigurationEncryption)} Started");

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
            logger.Information($"{nameof(AddConfigurationEncryption)} Finished");

        return true;
    }
}