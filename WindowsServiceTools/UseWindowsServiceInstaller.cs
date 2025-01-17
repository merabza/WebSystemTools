using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemToolsShared;
using WebInstallers;

namespace WindowsServiceTools;

// ReSharper disable once UnusedType.Global
public sealed class UseWindowsServiceInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public bool InstallServices(WebApplicationBuilder builder, bool debugMode, string[] args,
        Dictionary<string, string> parameters)
    {
        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Started");

        //ასე გადაკეთება საჭიროა იმისათვის, რომ შესაძლებელი იყოს
        //პროგრამის გაშვება, როგორც კონსოლიდან,
        //ისე როგორც სერვისი
        var isService = !(Debugger.IsAttached || args.ToList().Contains("--console"));

        if (isService && SystemStat.IsWindows())
            builder.Host.UseWindowsService();

        if (debugMode)
            Console.WriteLine($"{GetType().Name}.{nameof(InstallServices)} Finished");

        return true;
    }

    public bool UseServices(WebApplication app, bool debugMode)
    {
        return true;
    }
}