using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Serilog;
using SystemTools.SystemToolsShared;

namespace WindowsServiceTools;

// ReSharper disable once UnusedType.Global
public static class UseWindowsServiceHostBuilderExtensions
{
    public static bool UseWindowsServiceOnWindows(this IHostBuilder hostBuilder, ILogger? debugLogger, string[] args)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseWindowsServiceOnWindows));

        //ასე გადაკეთება საჭიროა იმისათვის, რომ შესაძლებელი იყოს
        //პროგრამის გაშვება, როგორც კონსოლიდან,
        //ისე როგორც სერვისი
        var isService = !(Debugger.IsAttached || args.ToList().Contains("--console"));

        if (isService && SystemStat.IsWindows())
            hostBuilder.UseWindowsService();

        debugLogger?.Information("{MethodName} Finished", nameof(UseWindowsServiceOnWindows));

        return true;
    }
}