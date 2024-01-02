using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using WebInstallers;

namespace SerilogLogger;

// ReSharper disable once UnusedType.Global
public sealed class SerilogLoggerInstaller : IInstaller
{
    //public void InstallUse(ConfigureHostBuilder configureHostBuilder)
    //{
    //    //Console.WriteLine("SerilogLoggerInstaller.InstallUse Started");

    //    //configureHostBuilder.UseSerilog();

    //    //Console.WriteLine("SerilogLoggerInstaller.InstallUse Finished");
    //}

    public int InstallPriority => 20;
    public int ServiceUsePriority => 20;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        //Console.WriteLine("SerilogLoggerInstaller.InstallServices Started");

        //როცა სერვისი გაშვებულია კონსოლის ცვლილებები პრობლემებს იწვევს
        //ამიტომ აქ ვამოწმებთ და კონსოლს ვეხებით მხოლოდ დებაგერით გაშვებისას
        if (Debugger.IsAttached)
            Console.OutputEncoding = Encoding.UTF8;

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
        //Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        LogSerilogFilePath(builder.Configuration);

        //Console.WriteLine("SerilogLoggerInstaller.InstallServices Finished");
        //Log.Information("Hello, world!");

        //Console.WriteLine("SerilogLoggerInstaller.InstallUse Started");

        builder.Host.UseSerilog();

        //Console.WriteLine("SerilogLoggerInstaller.InstallUse Finished");
    }

    public void UseServices(WebApplication app)
    {
    }


    //ეს მეთოდი წამოღებულია SystemToolsShared.StShared კლასიდან.
    //როცა შესაძლებელი იქნება ამ SystemToolsShared ბიბლიოთეკის მიერთება,
    //ამ მეთოდის გამოძახება უნდა მოხდეს ამ ბიბლიოთეკიდან
    //ამ რედაქციაში გათვალისწინებულია Nullable=enable
    private static void LogSerilogFilePath(IConfiguration config)
    {
        var serilogSettings = config.GetSection("Serilog");

        //if (serilogSettings == null)
        //{
        //    Console.WriteLine("Serilog settings not set");
        //    return;
        //}

        var writeToSection =
            serilogSettings.GetChildren().SingleOrDefault(s => s.Key == "WriteTo");

        if (writeToSection == null)
        {
            Console.WriteLine("Serilog WriteTo Section not set");
            return;
        }

        var writeToWithNameFile =
            writeToSection.GetChildren().FirstOrDefault(child => child["Name"] == "File");
        if (writeToWithNameFile == null)
        {
            Console.WriteLine("Serilog WriteTo File Section not set");
            return;
        }

        var argsSection = writeToWithNameFile.GetChildren().SingleOrDefault(s => s.Key == "Args");
        if (argsSection == null)
        {
            Console.WriteLine("Serilog WriteTo File Args Section not set");
            return;
        }

        var path = argsSection.GetChildren().SingleOrDefault(s => s.Key == "path");
        if (path == null)
        {
            Console.WriteLine("Serilog WriteTo File Args path not set");
            return;
        }

        //if ( path.Value is not null)
        //    FileStat.CreateFolderIfNotExists(path.Value, true);

        Console.WriteLine($"Serilog WriteTo File Path is: {path.Value}");
    }
}