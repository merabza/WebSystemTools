﻿using System;
using System.Collections.Generic;
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
    public int InstallPriority => 20;
    public int ServiceUsePriority => 20;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

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

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Finished");

        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        return true;
    }

    //ეს მეთოდი წამოღებულია SystemToolsShared.StShared კლასიდან.
    //როცა შესაძლებელი იქნება ამ SystemToolsShared ბიბლიოთეკის მიერთება,
    //ამ მეთოდის გამოძახება უნდა მოხდეს ამ ბიბლიოთეკიდან
    //ამ რედაქციაში გათვალისწინებულია Nullable=enable
    private static void LogSerilogFilePath(IConfiguration config)
    {
        var serilogSettings = config.GetSection("Serilog");

        //if (serilogSettings is null)
        //{
        //    Console.WriteLine("Serilog settings not set");
        //    return;
        //}

        var writeToSection = serilogSettings.GetChildren().SingleOrDefault(s => s.Key == "WriteTo");

        if (writeToSection is null)
        {
            Console.WriteLine("Serilog WriteTo Section not set");
            return;
        }

        var writeToWithNameFile = writeToSection.GetChildren().FirstOrDefault(child => child["Name"] == "File");
        if (writeToWithNameFile is null)
        {
            Console.WriteLine("Serilog WriteTo File Section not set");
            return;
        }

        var argsSection = writeToWithNameFile.GetChildren().SingleOrDefault(s => s.Key == "Args");
        if (argsSection is null)
        {
            Console.WriteLine("Serilog WriteTo File Args Section not set");
            return;
        }

        var path = argsSection.GetChildren().SingleOrDefault(s => s.Key == "path");
        if (path is null)
        {
            Console.WriteLine("Serilog WriteTo File Args path not set");
            return;
        }

        //if ( path.Value is not null)
        //    FileStat.CreateFolderIfNotExists(path.Value, true);

        Console.WriteLine($"Serilog WriteTo File Path is: {path.Value}");
    }
}