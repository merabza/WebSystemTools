using System;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SystemToolsShared;
using TestToolsData.Contracts.V1;
using TestToolsData.Models;
using WebInstallers;

namespace TestToolsApi.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public sealed class TestEndpoints : IInstaller
{
    public int InstallPriority => 70;
    public int ServiceUsePriority => 70;

    public void InstallServices(WebApplicationBuilder builder, string[] args)
    {
    }

    public void UseServices(WebApplication app)
    {
        //Console.WriteLine("TestToolsMini.UseServices Started");
        app.MapGet(TestApiRoutes.Test.TestConnection, Test);
        app.MapGet(TestApiRoutes.Test.GetIp, Ip);
        app.MapGet(TestApiRoutes.Test.GetVersion, Version);
        app.MapGet(TestApiRoutes.Test.GetAppSettingsVersion, AppSettingsVersion);
        app.MapGet(TestApiRoutes.Test.GetSettings, Settings);
        //Console.WriteLine("TestToolsMini.UseServices Finished");
    }

    //შესასვლელი წერტილი (endpoint)
    //დანიშნულება -> კავშირის შემოწმების საშუალება
    //შემავალი ინფორმაცია -> არა
    //უფლება -> შემოწმება საჭირო არ არის
    //მოქმედება -> უბრალოდ აბრუნებს 200 კოდს. თუ ამ მეთოდმა იმუშავა, კლიენტი მიხვდება, რომ პროგრამა გაშვებულია
    // GET api/v1/test/testconnection
    //[HttpGet(TestApiRoutes.Test.TestConnection)]
    private static IResult Test()
    {
        return Results.Ok(true);
    }


    // GET api/test/v1/getip
    //[HttpGet(TestApiRoutes.Test.GetIp)]
    private static IResult Ip(ILogger<TestEndpoints> logger, HttpRequest request)
    {
        var ret =
            $"{request.HttpContext.Connection.RemoteIpAddress} {Assembly.GetEntryAssembly()?.GetName().Version}";
        logger.LogInformation($"Test from {ret}");
        return Results.Ok(ret);
    }

    //// GET api/test/v1/getversion
    //[HttpGet(TestApiRoutes.Test.GetVersion)]
    private static IResult Version(ILogger<TestEndpoints> logger)
    {
        var ret = $"{Assembly.GetEntryAssembly()?.GetName().Version}";
        logger.LogInformation($"Version Test {ret}");
        return Results.Ok(ret);
    }

    // GET api/test/v1/getappsettingsversion
    //[HttpGet(TestApiRoutes.Test.GetAppSettingsVersion)]
    private static IResult AppSettingsVersion(ILogger<TestEndpoints> logger, IConfiguration config)
    {
        try
        {
            var versionInfo = VersionInfo.Create(config);
            var ret = versionInfo == null ? "Version not detected" : $"{versionInfo.AppSettingsVersion}";
            logger.LogInformation($"Version Test {ret}");
            return Results.Ok(ret);
        }
        catch (Exception e)
        {
            logger.LogError(e, null);
            throw;
        }
    }

    // GET api/test/v1/getsettings
    //[HttpGet(TestApiRoutes.Test.GetSettings)]
    private static IResult Settings()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Host name: {Environment.MachineName.Capitalize()}");
        sb.AppendLine($"Version: {Assembly.GetEntryAssembly()?.GetName().Version}");
        //ApiKeysDomain apiKeys = ApiKeysDomain.Create(_configuration, _logger);
        //sb.AppendLine($"View Api Keys. count is - {apiKeys.ApiKeys.Count}");
        //foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
        //{
        //    sb.AppendLine($"RemoteIpAddress is - {key.RemoteIpAddress}");
        //    sb.AppendLine($"ApiKey is - {key.ApiKey}");
        //}

        //sb.AppendLine("View Api Keys Finished");
        return Results.Ok(sb.ToString());
    }
}