using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using SystemTools.SystemToolsShared;

namespace WebSystemTools.ConfigurationEncrypt;

// ReSharper disable once ClassNeverInstantiated.Global
public static class ConfigurationEncryptHostBuilderExtensions
{
    public static bool AddConfigurationEncryption(this IConfigurationBuilder configurationBuilder, ILogger? debugLogger,
        string appKey)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddConfigurationEncryption));

        string key = appKey + Environment.MachineName.Capitalize();

        string pathToContentRoot = Directory.GetCurrentDirectory();

        if (!Debugger.IsAttached && SystemStat.IsWindows())
        {
            string? newPathToContentRoot = StShared.GetMainModulePath();
            if (newPathToContentRoot is not null)
            {
                pathToContentRoot = newPathToContentRoot;
            }

            Console.WriteLine("!Debugger.IsAttached && IsWindows() so pathToContentRoot=" + pathToContentRoot);
        }

        IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(pathToContentRoot)
            .AddEncryptedJsonFile(Path.Combine(pathToContentRoot, "appsettingsEncoded.json"), false, true, key,
                Path.Combine(pathToContentRoot, "appsetenkeys.json")).Build();
        configurationBuilder.AddConfiguration(config);

        debugLogger?.Information("{MethodName} Finished", nameof(AddConfigurationEncryption));

        return true;
    }
}
