using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SystemTools.ApiContracts;
using SystemTools.ApiKeysManagement;
using SystemTools.ApiKeysManagement.Domain;

namespace WebSystemTools.ApiKeyIdentity;

//https://dejanstojanovic.net/aspnet/2021/december/supporting-multiple-authentication-schemes-in-aspnet-core-webapi/
public sealed class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyFinder _apiKeyFinder;
    private readonly ILogger _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public TokenAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory, UrlEncoder encoder, IApiKeyFinder apiKeyFinder) : base(options, loggerFactory,
        encoder)
    {
        _logger = loggerFactory.CreateLogger<TokenAuthenticationHandler>();
        _apiKeyFinder = apiKeyFinder;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.HttpContext.User.Identity is null || Request.HttpContext.User.Identity.IsAuthenticated)
        {
            return await Task.FromResult(AuthenticateResult.NoResult());
        }

        string apiKey = Request.Query[ApiKeysConstants.ApiKeyParameterName].ToString();
        IPAddress? remoteAddress = Request.HttpContext.Connection.RemoteIpAddress;

        if (remoteAddress is null)
        {
            return await Task.FromResult(AuthenticateResult.NoResult());
        }

        string remoteIpAddress = remoteAddress.MapToIPv4().ToString();

        if (!await Check(apiKey, remoteIpAddress))
        {
            return await Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new Claim[] { new(ClaimTypes.Name, remoteIpAddress) };
        var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        var user = new GenericPrincipal(claimsIdentity,
            claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray());

        Request.HttpContext.User = user;

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private async ValueTask<bool> Check(string? apiKey, string remoteIpAddress)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        //var apiKeys = ApiKeysDomain.Create(_apiKeyFinder, _logger);

        //_logger.LogInformation($"View Api Keys. count is - {apiKeys.ApiKeys.Count}");
        //foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
        //{
        //    _logger.LogInformation($"RemoteIpAddress is - {key.RemoteIpAddress}");
        //    _logger.LogInformation($"ApiKey is - {key.ApiKey}");
        //}
        //_logger.LogInformation("View Api Keys Finished");

        ApiKeyAndRemoteIpAddressDomain? ak = await _apiKeyFinder.GetApiKeyAndRemAddress(apiKey, remoteIpAddress);

        if (ak != null)
        {
            return true;
        }

        _logger.LogError("RemoteIpAddress is - {RemoteIpAddress}", remoteIpAddress);
        _logger.LogError("API Key is invalid - {ApiKey}", apiKey);
        return false;
    }
}
