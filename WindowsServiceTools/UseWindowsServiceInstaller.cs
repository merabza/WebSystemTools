using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using SystemToolsShared;
using WebInstallers;

namespace WindowsServiceTools;

// ReSharper disable once UnusedType.Global
public sealed class UseWindowsServiceInstaller : IInstaller
{
    public int InstallPriority => 30;
    public int ServiceUsePriority => 30;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
        //Console.WriteLine("WindowsServiceInstaller.InstallServices Started");

        //ასე გადაკეთება საჭიროა იმისათვის, რომ შესაძლებელი იყოს
        //პროგრამის გაშვება, როგორც კონსოლიდან,
        //ისე როგორც სერვისი
        var isService = !(Debugger.IsAttached || args.ToList().Contains("--console"));

        if (isService && SystemStat.IsWindows())
            builder.Host.UseWindowsService();
        //Console.WriteLine("WindowsServiceInstaller.InstallServices Finished");
    }

    public void UseServices(WebApplication app)
    {
    }
}