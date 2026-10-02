using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SystemTools.ApiKeysManagement;
using SystemTools.ApiKeysManagement.Domain;
using WebSystemTools.ApiKeyIdentity.Tests.TestDoubles;

namespace WebSystemTools.ApiKeyIdentity.Tests;

//The handler takes the key from the ApiKey query parameter and the client address from the connection
public sealed class TokenAuthenticationHandlerTests
{
    private const string ApiKey = "test-key-123";
    private const string ClientAddress = "10.1.2.3";

    private static Mock<IApiKeyFinder> CreateFinder(bool knowsTheKey)
    {
        var finder = new Mock<IApiKeyFinder>();
        finder.Setup(f => f.GetApiKeyAndRemAddress(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(knowsTheKey
            ? new ApiKeyAndRemoteIpAddressDomain { ApiKey = ApiKey, RemoteIpAddress = ClientAddress }
            : null);
        return finder;
    }

    private static DefaultHttpContext CreateContext(string? queryString, string? remoteAddress = ClientAddress)
    {
        var context = new DefaultHttpContext();
        if (queryString is not null)
        {
            context.Request.QueryString = new QueryString(queryString);
        }

        if (remoteAddress is not null)
        {
            context.Connection.RemoteIpAddress = IPAddress.Parse(remoteAddress);
        }

        return context;
    }

    private static async Task<AuthenticateResult> Authenticate(HttpContext context, IApiKeyFinder finder,
        ILoggerFactory? loggerFactory = null)
    {
        var options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
        options.Setup(o => o.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());
        var handler = new TokenAuthenticationHandler(options.Object, loggerFactory ?? NullLoggerFactory.Instance,
            UrlEncoder.Default, finder);
        await handler.InitializeAsync(
            new AuthenticationScheme(AuthenticationSchemaNames.ApiKeyAuthentication, null,
                typeof(TokenAuthenticationHandler)), context);
        return await handler.AuthenticateAsync();
    }

    [Fact]
    public async Task Authenticate_Succeeds_WithAKeyThatTheFinderKnows()
    {
        Mock<IApiKeyFinder> finder = CreateFinder(true);
        DefaultHttpContext context = CreateContext($"?ApiKey={ApiKey}");

        AuthenticateResult result = await Authenticate(context, finder.Object);

        Assert.True(result.Succeeded);
        Assert.Equal(AuthenticationSchemaNames.ApiKeyAuthentication, result.Ticket.AuthenticationScheme);
        Assert.Equal(ClientAddress, result.Principal.Identity?.Name);
        Assert.True(context.User.Identity?.IsAuthenticated);
        Assert.Equal(ClientAddress, context.User.Identity?.Name);
        finder.Verify(f => f.GetApiKeyAndRemAddress(ApiKey, ClientAddress), Times.Once);
    }

    //The principal carries only the name claim (the client address): an API key grants no role
    [Fact]
    public async Task Authenticate_GivesTheUserNoRole()
    {
        DefaultHttpContext context = CreateContext($"?apikey={ApiKey}");

        await Authenticate(context, CreateFinder(true).Object);

        Assert.False(context.User.IsInRole(ClientAddress));
    }

    //ApiClient sends the parameter in lower case, and the query is read case-insensitively
    [Fact]
    public async Task Authenticate_ReadsTheKeyFromALowerCaseParameter()
    {
        Mock<IApiKeyFinder> finder = CreateFinder(true);

        AuthenticateResult result = await Authenticate(CreateContext($"?apikey={ApiKey}"), finder.Object);

        Assert.True(result.Succeeded);
        finder.Verify(f => f.GetApiKeyAndRemAddress(ApiKey, ClientAddress), Times.Once);
    }

    [Fact]
    public async Task Authenticate_DecodesAnEscapedKey()
    {
        Mock<IApiKeyFinder> finder = CreateFinder(true);

        await Authenticate(CreateContext("?apikey=a%2Bb%26c%3Dd"), finder.Object);

        finder.Verify(f => f.GetApiKeyAndRemAddress("a+b&c=d", ClientAddress), Times.Once);
    }

    [Fact]
    public async Task Authenticate_PassesTheIPv4FormOfAMappedClientAddress()
    {
        Mock<IApiKeyFinder> finder = CreateFinder(true);

        await Authenticate(CreateContext($"?apikey={ApiKey}", "::ffff:10.1.2.3"), finder.Object);

        finder.Verify(f => f.GetApiKeyAndRemAddress(ApiKey, ClientAddress), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("?apikey=")]
    [InlineData("?apikey=%20%20")]
    [InlineData("?key=test-key-123")]
    public async Task Authenticate_ReturnsNoResult_WithoutKey(string? queryString)
    {
        Mock<IApiKeyFinder> finder = CreateFinder(true);

        AuthenticateResult result = await Authenticate(CreateContext(queryString), finder.Object);

        Assert.True(result.None);
        finder.Verify(f => f.GetApiKeyAndRemAddress(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Authenticate_ReturnsNoResult_WithoutClientAddress()
    {
        Mock<IApiKeyFinder> finder = CreateFinder(true);

        AuthenticateResult result = await Authenticate(CreateContext($"?apikey={ApiKey}", null), finder.Object);

        Assert.True(result.None);
        finder.Verify(f => f.GetApiKeyAndRemAddress(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Authenticate_ReturnsNoResult_WhenTheUserIsAlreadyAuthenticated()
    {
        Mock<IApiKeyFinder> finder = CreateFinder(true);
        DefaultHttpContext context = CreateContext($"?apikey={ApiKey}");
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "someone")], "Other"));

        AuthenticateResult result = await Authenticate(context, finder.Object);

        Assert.True(result.None);
        finder.Verify(f => f.GetApiKeyAndRemAddress(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Authenticate_ReturnsNoResult_ForAnUnknownKey()
    {
        DefaultHttpContext context = CreateContext($"?apikey={ApiKey}");

        AuthenticateResult result = await Authenticate(context, CreateFinder(false).Object);

        Assert.True(result.None);
        Assert.False(context.User.Identity?.IsAuthenticated);
    }

    //A refused key may differ from a real one by a single character, so only its length is logged
    [Fact]
    public async Task Authenticate_LogsTheAddressAndTheLengthOfAnUnknownKey_ButNotTheKey()
    {
        using var loggerFactory = new CollectingLoggerFactory();

        await Authenticate(CreateContext($"?apikey={ApiKey}"), CreateFinder(false).Object, loggerFactory);

        List<string> errors = [.. loggerFactory.Entries.Where(e => e.Level == LogLevel.Error).Select(e => e.Message)];
        Assert.Equal(["API Key is invalid. RemoteIpAddress is 10.1.2.3, API Key length is 12"], errors);
        Assert.DoesNotContain(loggerFactory.Entries, e => e.Message.Contains(ApiKey, StringComparison.Ordinal));
    }

    [Fact]
    public async Task Authenticate_LogsNothing_WhenTheErrorLevelIsDisabled()
    {
        using var loggerFactory = new CollectingLoggerFactory(false);

        await Authenticate(CreateContext($"?apikey={ApiKey}"), CreateFinder(false).Object, loggerFactory);

        Assert.Empty(loggerFactory.Entries);
    }

    [Fact]
    public async Task Authenticate_LogsNoError_ForAKnownKey()
    {
        using var loggerFactory = new CollectingLoggerFactory();

        await Authenticate(CreateContext($"?apikey={ApiKey}"), CreateFinder(true).Object, loggerFactory);

        Assert.DoesNotContain(loggerFactory.Entries, e => e.Level == LogLevel.Error);
    }
}
