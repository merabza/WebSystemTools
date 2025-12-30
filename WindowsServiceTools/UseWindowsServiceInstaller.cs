using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Hosting;
using SystemToolsShared;

namespace WindowsServiceTools;

// ReSharper disable once UnusedType.Global
public static class UseWindowsServiceInstaller
{
    public static bool UseWindowsServiceOnWindows(this IHostBuilder hostBuilder, bool debugMode, string[] args)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(UseWindowsServiceOnWindows)} Started");

        //ასე გადაკეთება საჭიროა იმისათვის, რომ შესაძლებელი იყოს
        //პროგრამის გაშვება, როგორც კონსოლიდან,
        //ისე როგორც სერვისი
        var isService = !(Debugger.IsAttached || args.ToList().Contains("--console"));

        if (isService && SystemStat.IsWindows())
            hostBuilder.UseWindowsService();

        if (debugMode)
            Console.WriteLine($"{nameof(UseWindowsServiceOnWindows)} Finished");

        return true;
    }
}