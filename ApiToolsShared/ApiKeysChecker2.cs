//using ApiToolsShared.Domain;
//using LanguageExt;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using SystemToolsShared;

//namespace ApiToolsShared;

//public sealed class ApiKeysChecker2
//{
//    private readonly IConfiguration _configuration;
//    private readonly HttpRequest _httpRequest;
//    private readonly ILogger _logger;

//    public ApiKeysChecker2(ILogger logger, IConfiguration configuration, HttpRequest httpRequest)
//    {
//        _logger = logger;
//        _configuration = configuration;
//        _httpRequest = httpRequest;
//    }

//    public bool Check(string? apiKey, string remoteIpAddress)
//    {
//        if (string.IsNullOrWhiteSpace(apiKey))
//            return false;

//        var apiKeys = ApiKeysDomain.Create(_configuration, _logger);

//        //_logger.LogInformation($"View Api Keys. count is - {apiKeys.ApiKeys.Count}");
//        //foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
//        //{
//        //    _logger.LogInformation($"RemoteIpAddress is - {key.RemoteIpAddress}");
//        //    _logger.LogInformation($"ApiKey is - {key.ApiKey}");
//        //}
//        //_logger.LogInformation("View Api Keys Finished");

//        var ak = apiKeys.GetAppSettingsByApiKey(apiKey, remoteIpAddress);


//        if (ak != null)
//            return true;
//        _logger.LogError("RemoteIpAddress is - {remoteIpAddress}", remoteIpAddress);
//        _logger.LogError("API Key is invalid - {apiKey}", apiKey);
//        return false;
//    }


//    //public async Task<T?> DeserializeAsync<T>(Stream bodyStream)
//    //{
//    //    using StreamReader reader = new(bodyStream);
//    //    var body = await reader.ReadToEndAsync();
//    //    return JsonConvert.DeserializeObject<T>(body);
//    //}

//    public Option<Err> Check()
//    {
//        var apiKey = _httpRequest.HttpContext.Request.Query["ApiKey"].ToString();
//        var remoteAddress = _httpRequest.HttpContext.Connection.RemoteIpAddress;

//        if (remoteAddress is null)
//            return ApiErrors.InvalidRemoteAddress;

//        //var apiKeysChecker = new ApiKeysChecker(_logger, _configuration);

//        return !Check(apiKey, remoteAddress.MapToIPv4().ToString()) ? ApiErrors.ApiKeyIsInvalid : new Option<Err>();
//    }
//}

