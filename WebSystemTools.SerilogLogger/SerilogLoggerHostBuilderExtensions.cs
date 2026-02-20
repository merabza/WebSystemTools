using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WebSystemTools.SerilogLogger;

// ReSharper disable once UnusedType.Global
public static class SerilogLoggerHostBuilderExtensions
{
    public static ILogger UseSerilogLogger(this IHostBuilder hostBuilder, bool debugMode, IConfiguration configuration)
    {
        if (debugMode)
        {
            Console.WriteLine($"{nameof(UseSerilogLogger)} Started");
        }

        //როცა სერვისი გაშვებულია კონსოლის ცვლილებები პრობლემებს იწვევს
        //ამიტომ აქ ვამოწმებთ და კონსოლს ვეხებით მხოლოდ დებაგერით გაშვებისას
        if (Debugger.IsAttached)
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        //Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        LogSerilogFilePath(configuration);

        hostBuilder.UseSerilog();

        if (debugMode)
        {
            Console.WriteLine($"{nameof(UseSerilogLogger)} Finished");
        }

        return Log.Logger;
    }

    //ეს მეთოდი წამოღებულია SystemToolsShared.StShared კლასიდან.
    //როცა შესაძლებელი იქნება ამ SystemToolsShared ბიბლიოთეკის მიერთება,
    //ამ მეთოდის გამოძახება უნდა მოხდეს ამ ბიბლიოთეკიდან
    //ამ რედაქციაში გათვალისწინებულია Nullable=enable
    private static void LogSerilogFilePath(IConfiguration config)
    {
        IConfigurationSection serilogSettings = config.GetSection("Serilog");

        IConfigurationSection? writeToSection = serilogSettings.GetChildren().SingleOrDefault(s => s.Key == "WriteTo");

        if (writeToSection is null)
        {
            Console.WriteLine("Serilog WriteTo Section not set");
            return;
        }

        IConfigurationSection? writeToWithNameFile =
            writeToSection.GetChildren().FirstOrDefault(child => child["Name"] == "File");
        if (writeToWithNameFile is null)
        {
            Console.WriteLine("Serilog WriteTo File Section not set");
            return;
        }

        IConfigurationSection? argsSection = writeToWithNameFile.GetChildren().SingleOrDefault(s => s.Key == "Args");
        if (argsSection is null)
        {
            Console.WriteLine("Serilog WriteTo File Args Section not set");
            return;
        }

        IConfigurationSection? path = argsSection.GetChildren().SingleOrDefault(s => s.Key == "path");
        if (path is null)
        {
            Console.WriteLine("Serilog WriteTo File Args path not set");
            return;
        }

        Console.WriteLine($"Serilog WriteTo File Path is: {path.Value}");
    }
}
