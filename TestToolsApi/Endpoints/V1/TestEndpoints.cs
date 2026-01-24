using System;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SystemTools.SystemToolsShared;
using SystemTools.TestApiContracts.V1.Routes;
using TestToolsData.Models;
using ILogger = Serilog.ILogger;

namespace TestToolsApi.Endpoints.V1;

// ReSharper disable once UnusedType.Global
public static class TestEndpoints
{
    public static bool UseTestEndpoints(this IEndpointRouteBuilder endpoints, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(UseTestEndpoints));

        var group = endpoints.MapGroup(TestApiRoutes.ApiBase + TestApiRoutes.Test.TestBase);

        group.MapGet(TestApiRoutes.Test.TestConnection, Test);
        group.MapGet(TestApiRoutes.Test.GetIp, GetIp);
        group.MapGet(TestApiRoutes.Test.GetVersion, Version);
        group.MapGet(TestApiRoutes.Test.GetAppSettingsVersion, AppSettingsVersion);
        group.MapGet(TestApiRoutes.Test.GetSettings, Settings);

        debugLogger?.Information("{MethodName} Finished", nameof(UseTestEndpoints));

        return true;
    }

    //შესასვლელი წერტილი (endpoint)
    //დანიშნულება -> კავშირის შემოწმების საშუალება
    //შემავალი ინფორმაცია -> არა
    //უფლება -> შემოწმება საჭირო არ არის
    //მოქმედება -> უბრალოდ აბრუნებს 200 კოდს. თუ ამ მეთოდმა იმუშავა, კლიენტი მიხვდება, რომ პროგრამა გაშვებულია
    // GET api/v1/test/testconnection
    //[HttpGet(TestApiRoutes.Test.TestConnection)]
    private static Ok<bool> Test()
    {
        return TypedResults.Ok(true);
    }

    // GET api/v1/test/getip
    private static IResult GetIp([FromServices] ILogger<Api> logger, HttpRequest request)
    {
        var ret = $"{request.HttpContext.Connection.RemoteIpAddress} {Assembly.GetEntryAssembly()?.GetName().Version}";
        logger.LogInformation("Test from {ret}", ret);
        return Results.Text(ret, "text/plain");
    }

    // GET api/v1/test/getversion
    private static IResult Version([FromServices] ILogger<Api> logger)
    {
        var ret = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
        logger.LogInformation("Version Test {ret}", ret);
        return Results.Text(ret ?? string.Empty, "text/plain");
    }

    // GET api/v1/test/getappsettingsversion
    //[HttpGet(TestApiRoutes.Test.GetAppSettingsVersion)]
    private static IResult AppSettingsVersion([FromServices] ILogger<Api> logger, IConfiguration config)
    {
        var versionInfo = VersionInfo.Create(config);
        var ret = versionInfo is null ? "Version not detected" : versionInfo.AppSettingsVersion;
        logger.LogInformation("Version Test {ret}", ret);
        return Results.Text(ret ?? string.Empty, "text/plain");
    }

    // GET api/v1/test/getsettings
    //[HttpGet(TestApiRoutes.Test.GetSettings)]
    private static IResult Settings()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Host name: {Environment.MachineName.Capitalize()}");
        sb.AppendLine($"Version: {Assembly.GetEntryAssembly()?.GetName().Version}");
        //if (_debugMode)
        //{
        //    ApiKeysDomain apiKeys = ApiKeysDomain.Create(_configuration, _logger);
        //    sb.AppendLine($"View Api Keys. count is - {apiKeys.ApiKeys.Count}");
        //    foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
        //    {
        //        sb.AppendLine($"RemoteIpAddress is - {key.RemoteIpAddress}");
        //        sb.AppendLine($"ApiKey is - {key.ApiKey}");
        //    }

        //    sb.AppendLine("View Api Keys Finished");
        //}

        return Results.Text(sb.ToString(), "text/plain");
    }
}