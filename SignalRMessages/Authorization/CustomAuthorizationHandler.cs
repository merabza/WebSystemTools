//using System.Threading.Tasks;
//using ApiToolsShared;
//using ApiToolsShared.Domain;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

//namespace LibWebAgentMessages.Authorization;

////https://dejanstojanovic.net/aspnet/2020/march/custom-signalr-hub-authorization-in-aspnet-core/
//public class CustomAuthorizationHandler : AuthorizationHandler<CustomAuthorizationRequirement>
//{
//    readonly IHttpContextAccessor _httpContextAccessor;

//    public CustomAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
//    {
//        _httpContextAccessor = httpContextAccessor;
//    }

//    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
//        CustomAuthorizationRequirement requirement)
//    {
//        if (_httpContextAccessor.HttpContext is null)
//        {
//            context.Fail();
//            return Task.CompletedTask;
//        }

//        if (_httpContextAccessor.HttpContext.User.Identity is null)
//        {
//            context.Fail();
//            return Task.CompletedTask;
//        }

//        if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
//        {
//            context.Fail();
//            return Task.CompletedTask;
//        }


//        //var apiKey = _httpContextAccessor.HttpContext.Request.Query["ApiKey"].ToString();
//        //var remoteAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

//        //if (remoteAddress is null)
//        //{
//        //    context.Fail();
//        //    return Task.CompletedTask;
//        //}

//        //if (!Check(apiKey, remoteAddress.MapToIPv4().ToString()))
//        //{
//        //    context.Fail();
//        //    return Task.CompletedTask;
//        //}

//        context.Succeed(requirement);
//        // Return completed task  
//        return Task.CompletedTask;
//    }

//    //private bool Check(string? apiKey, string remoteIpAddress)
//    //{
//    //    if (string.IsNullOrWhiteSpace(apiKey))
//    //        return false;

//    //    var apiKeys = ApiKeysDomain.Create(_configuration, _logger);

//    //    //_logger.LogInformation($"View Api Keys. count is - {apiKeys.ApiKeys.Count}");
//    //    //foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
//    //    //{
//    //    //    _logger.LogInformation($"RemoteIpAddress is - {key.RemoteIpAddress}");
//    //    //    _logger.LogInformation($"ApiKey is - {key.ApiKey}");
//    //    //}
//    //    //_logger.LogInformation("View Api Keys Finished");

//    //    var ak = apiKeys.AppSettingsByApiKey(apiKey, remoteIpAddress);


//    //    if (ak != null)
//    //        return true;
//    //    _logger.LogError("RemoteIpAddress is - {remoteIpAddress}", remoteIpAddress);
//    //    _logger.LogError("API Key is invalid - {apiKey}", apiKey);
//    //    return false;
//    //}
//}

