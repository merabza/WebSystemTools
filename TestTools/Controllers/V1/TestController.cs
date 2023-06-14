using System;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SystemToolsShared;
using TestToolsData.Contracts.V1;
using TestToolsData.Models;

namespace TestTools.Controllers.V1;

public class TestController : Controller
{
    private readonly IConfiguration _configuration;

    private readonly ILogger<TestController> _logger;


    public TestController(ILogger<TestController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }


    //შესასვლელი წერტილი (endpoint)
    //დანიშნულება -> კავშირის შემოწმების საშუალება
    //შემავალი ინფორმაცია -> არა
    //უფლება -> შემოწმება საჭირო არ არის
    //მოქმედება -> უბრალოდ აბრუნებს 200 კოდს. თუ ამ მეთოდმა იმუშავა, კლიენტი მიხვდება, რომ პროგრამა გაშვებულია
    // GET api/v1/test/testconnection
    [HttpGet(TestApiRoutes.Test.TestConnection)]
    public ActionResult<bool> Test()
    {
        return Ok(true);
    }


    // GET api/test/v1/getip
    [HttpGet(TestApiRoutes.Test.GetIp)]
    public string GetIp()
    {
        var ret =
            $"{Request.HttpContext.Connection.RemoteIpAddress} {Assembly.GetEntryAssembly()?.GetName().Version}";
        _logger.LogInformation($"Test from {ret}");
        return ret;
    }

    // GET api/test/v1/getversion
    [HttpGet(TestApiRoutes.Test.GetVersion)]
    public string GetVersion()
    {
        var ret = $"{Assembly.GetEntryAssembly()?.GetName().Version}";
        _logger.LogInformation($"Version Test {ret}");
        return ret;
    }

    // GET api/test/v1/getappsettingsversion
    [HttpGet(TestApiRoutes.Test.GetAppSettingsVersion)]
    public string GetAppSettingsVersion()
    {
        try
        {
            var versionInfo = VersionInfo.Create(_configuration, _logger);
            var ret = versionInfo == null ? "Version not detected" : $"{versionInfo.AppSettingsVersion}";
            _logger.LogInformation($"Version Test {ret}");
            return ret;
        }
        catch (Exception e)
        {
            _logger.LogError(e, null);
            throw;
        }
    }

    // GET api/test/v1/getsettings
    [HttpGet(TestApiRoutes.Test.GetSettings)]
    public string GetSettings()
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

        return sb.ToString();
    }
}