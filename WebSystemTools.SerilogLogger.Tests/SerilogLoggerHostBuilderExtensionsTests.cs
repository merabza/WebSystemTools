using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace WebSystemTools.SerilogLogger.Tests;

//UseSerilogLogger sets the static Serilog logger and writes to the console. The tests of one class run one after
//another, and no other class of this project uses either of them
public sealed class SerilogLoggerHostBuilderExtensionsTests : IDisposable
{
    private const string Key = "made-up-key-456";

    private readonly string _folder =
        Path.Combine(Path.GetTempPath(), $"WebSystemTools.SerilogLogger.Tests.{Guid.NewGuid():N}");

    public SerilogLoggerHostBuilderExtensionsTests()
    {
        Directory.CreateDirectory(_folder);
    }

    private string LogFile => Path.Combine(_folder, "test.log");

    public void Dispose()
    {
        Log.CloseAndFlush();
        Directory.Delete(_folder, true);
    }

    private Dictionary<string, string?> FileSinkSettings()
    {
        return new Dictionary<string, string?>
        {
            ["Serilog:Using:0"] = "Serilog.Sinks.File",
            ["Serilog:WriteTo:0:Name"] = "File",
            ["Serilog:WriteTo:0:Args:path"] = LogFile
        };
    }

    private static (Serilog.ILogger Logger, string Output) UseSerilogLogger(IHostBuilder hostBuilder, bool debugMode,
        Dictionary<string, string?> settings)
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        TextWriter original = Console.Out;
        using var writer = new StringWriter(CultureInfo.InvariantCulture);
        Console.SetOut(writer);
        try
        {
            Serilog.ILogger logger = hostBuilder.UseSerilogLogger(debugMode, configuration);
            return (logger, writer.ToString());
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    //The request log of ASP.NET Core reaches Serilog through Microsoft.Extensions.Logging of the host
    [Fact]
    public void UseSerilogLogger_HidesTheApiKeyInEveryLogOfTheHost()
    {
        var hostBuilder = new HostBuilder();
        (Serilog.ILogger logger, _) = UseSerilogLogger(hostBuilder, false, FileSinkSettings());
        using (IHost host = hostBuilder.Build())
        {
            Microsoft.Extensions.Logging.ILogger hostingLogger = host.Services.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Microsoft.AspNetCore.Hosting.Diagnostics");
            if (hostingLogger.IsEnabled(LogLevel.Information))
            {
                hostingLogger.LogInformation("Request starting {Method} {Path}{QueryString}", "GET",
                    "/api/v1/runtimes", "?apikey=" + Key + "&version=2");
            }
        }

        logger.Information("Hub {QueryString}", "?ApiKey=" + Key + "&id=abc");
        Log.CloseAndFlush();

        string log = File.ReadAllText(LogFile);
        Assert.DoesNotContain(Key, log, StringComparison.Ordinal);
        Assert.Contains("Request starting GET /api/v1/runtimes?apikey=***&version=2", log, StringComparison.Ordinal);
        Assert.Contains("Hub ?ApiKey=***&id=abc", log, StringComparison.Ordinal);
    }

    [Fact]
    public void UseSerilogLogger_ReturnsTheStaticLoggerAndMakesSerilogTheLoggerFactoryOfTheHost()
    {
        var hostBuilder = new HostBuilder();

        (Serilog.ILogger logger, _) = UseSerilogLogger(hostBuilder, false, FileSinkSettings());

        Assert.Same(Log.Logger, logger);
        using IHost host = hostBuilder.Build();
        Assert.IsType<SerilogLoggerFactory>(host.Services.GetRequiredService<ILoggerFactory>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UseSerilogLogger_WritesTheStartAndTheEndOnlyInDebugMode(bool debugMode)
    {
        (_, string output) = UseSerilogLogger(new HostBuilder(), debugMode, FileSinkSettings());

        Assert.Equal(debugMode, output.Contains("UseSerilogLogger Started", StringComparison.Ordinal));
        Assert.Equal(debugMode, output.Contains("UseSerilogLogger Finished", StringComparison.Ordinal));
    }

    [Fact]
    public void UseSerilogLogger_WritesThePathOfTheFileSink()
    {
        (_, string output) = UseSerilogLogger(new HostBuilder(), false, FileSinkSettings());

        Assert.Contains($"Serilog WriteTo File Path is: {LogFile}", output, StringComparison.Ordinal);
    }

    //Each setting is missing one more part of the path of the file sink
    public static TheoryData<string, string, string> IncompleteFileSinkSettings => new()
    {
        { "Serilog:MinimumLevel", "Information", "Serilog WriteTo Section not set" },
        { "Serilog:WriteTo:0:Name", "Console", "Serilog WriteTo File Section not set" },
        { "Serilog:WriteTo:0:Name", "File", "Serilog WriteTo File Args Section not set" }
    };

    [Theory]
    [MemberData(nameof(IncompleteFileSinkSettings))]
    public void UseSerilogLogger_WritesWhichPartOfTheFileSinkIsMissing(string setting, string value,
        string expectedLine)
    {
        (_, string output) = UseSerilogLogger(new HostBuilder(), false,
            new Dictionary<string, string?> { [setting] = value });

        Assert.Contains(expectedLine, output, StringComparison.Ordinal);
    }

    [Fact]
    public void UseSerilogLogger_WritesThatThePathOfTheFileSinkIsMissing()
    {
        (_, string output) = UseSerilogLogger(new HostBuilder(), false,
            new Dictionary<string, string?>
            {
                ["Serilog:WriteTo:0:Name"] = "File", ["Serilog:WriteTo:0:Args:rollingInterval"] = "Day"
            });

        Assert.Contains("Serilog WriteTo File Args path not set", output, StringComparison.Ordinal);
    }
}
