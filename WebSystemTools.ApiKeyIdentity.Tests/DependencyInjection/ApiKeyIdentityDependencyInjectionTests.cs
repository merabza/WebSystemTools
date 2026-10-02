using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using SystemTools.ApiKeysManagement;
using WebSystemTools.ApiKeyIdentity.DependencyInjection;

namespace WebSystemTools.ApiKeyIdentity.Tests.DependencyInjection;

//The services of AddApiKeyIdentity and the pipeline that UseApiKeysAuthorization builds on a real WebApplication.
//UseApiKeysAuthorization calls only UseAuthorization: WebApplication adds UseAuthentication by itself,
//because AddApiKeyIdentity registers the authentication services
public sealed class ApiKeyIdentityDependencyInjectionTests
{
    private const string ApiKey = "test-key-123";
    private const string ClientAddress = "10.1.2.3";
    private const string ProtectedPath = "/protected";
    private const string OpenPath = "/open";

    private static async Task<WebApplication> StartApplication()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ApiKeys:AppSettingsByApiKey:0:ApiKey"] = ApiKey,
            ["ApiKeys:AppSettingsByApiKey:0:RemoteIpAddress"] = ClientAddress
        });
        builder.Services.AddApiKeyIdentity(null);

        WebApplication app = builder.Build();
        app.UseApiKeysAuthorization(null);
        app.MapGet(ProtectedPath, () => "protected").RequireAuthorization();
        app.MapGet(OpenPath, () => "open");
        await app.StartAsync();
        return app;
    }

    //TestServer leaves the client address empty, so every request sets it
    private static async Task<int> Get(WebApplication app, string path, string? apiKey,
        string remoteAddress = ClientAddress)
    {
        HttpContext context = await app.GetTestServer().SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Get;
            c.Request.Path = path;
            if (apiKey is not null)
            {
                c.Request.QueryString = QueryString.Create("apikey", apiKey);
            }

            c.Connection.RemoteIpAddress = IPAddress.Parse(remoteAddress);
        });
        return context.Response.StatusCode;
    }

    [Fact]
    public void AddApiKeyIdentity_RegistersTheKeyFinderTheCurrentUserAndTheSignalRUserIdProvider()
    {
        var services = new ServiceCollection();

        IServiceCollection returned = services.AddApiKeyIdentity(null);

        Assert.Same(services, returned);
        Assert.Contains(services,
            d => d.ServiceType == typeof(IApiKeyFinder) && d.ImplementationType == typeof(ApiKeyByConfigFinder) &&
                 d.Lifetime == ServiceLifetime.Scoped);
        Assert.Contains(services,
            d => d.ServiceType == typeof(ICurrentUserByApiKey) &&
                 d.ImplementationType == typeof(CurrentUserByApiKey) && d.Lifetime == ServiceLifetime.Scoped);
        Assert.Contains(services,
            d => d.ServiceType == typeof(IUserIdProvider) && d.ImplementationType == typeof(CustomUserIdProvider) &&
                 d.Lifetime == ServiceLifetime.Singleton);
        Assert.Contains(services,
            d => d.ServiceType == typeof(IHttpContextAccessor) && d.Lifetime == ServiceLifetime.Singleton);
    }

    //Only DefaultAuthenticateScheme is set; the challenge (401) uses the only registered scheme
    [Fact]
    public async Task AddApiKeyIdentity_MakesTheApiKeySchemeTheDefaultForAuthenticationAndChallenge()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddApiKeyIdentity(null);
        await using WebApplication app = builder.Build();
        var schemes = app.Services.GetRequiredService<IAuthenticationSchemeProvider>();

        AuthenticationOptions options = app.Services.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        AuthenticationScheme? scheme = await schemes.GetSchemeAsync(AuthenticationSchemaNames.ApiKeyAuthentication);
        AuthenticationScheme? challengeScheme = await schemes.GetDefaultChallengeSchemeAsync();

        Assert.Equal(AuthenticationSchemaNames.ApiKeyAuthentication, options.DefaultAuthenticateScheme);
        Assert.Equal(typeof(TokenAuthenticationHandler), scheme?.HandlerType);
        Assert.Equal(AuthenticationSchemaNames.ApiKeyAuthentication, challengeScheme?.Name);
    }

    [Fact]
    public void AddApiKeyIdentity_LogsTheStartAndTheEnd()
    {
        var logger = new Mock<ILogger>();

        new ServiceCollection().AddApiKeyIdentity(logger.Object);

        logger.Verify(l => l.Information("{MethodName} Started", "AddApiKeyIdentity"), Times.Once);
        logger.Verify(l => l.Information("{MethodName} Finished", "AddApiKeyIdentity"), Times.Once);
    }

    [Fact]
    public async Task UseApiKeysAuthorization_ReturnsTrueAndLogsTheStartAndTheEnd()
    {
        var logger = new Mock<ILogger>();
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddApiKeyIdentity(null);
        await using WebApplication app = builder.Build();

        bool used = app.UseApiKeysAuthorization(logger.Object);

        Assert.True(used);
        logger.Verify(l => l.Information("{MethodName} Started", "UseApiKeysAuthorization"), Times.Once);
        logger.Verify(l => l.Information("{MethodName} Finished", "UseApiKeysAuthorization"), Times.Once);
    }

    [Fact]
    public async Task Pipeline_ReturnsUnauthorized_ForAProtectedEndpointWithoutKey()
    {
        await using WebApplication app = await StartApplication();

        Assert.Equal(StatusCodes.Status401Unauthorized, await Get(app, ProtectedPath, null));
    }

    [Fact]
    public async Task Pipeline_ReturnsUnauthorized_ForAProtectedEndpointWithAnInvalidKey()
    {
        await using WebApplication app = await StartApplication();

        Assert.Equal(StatusCodes.Status401Unauthorized, await Get(app, ProtectedPath, "wrong-key"));
    }

    [Fact]
    public async Task Pipeline_ReturnsUnauthorized_ForAProtectedEndpointWithAKeyFromAnotherAddress()
    {
        await using WebApplication app = await StartApplication();

        Assert.Equal(StatusCodes.Status401Unauthorized, await Get(app, ProtectedPath, ApiKey, "10.9.9.9"));
    }

    [Fact]
    public async Task Pipeline_ReturnsOk_ForAProtectedEndpointWithTheKeyOfTheAddress()
    {
        await using WebApplication app = await StartApplication();

        Assert.Equal(StatusCodes.Status200OK, await Get(app, ProtectedPath, ApiKey));
    }

    [Fact]
    public async Task Pipeline_ReturnsOk_ForAnOpenEndpointWithoutKey()
    {
        await using WebApplication app = await StartApplication();

        Assert.Equal(StatusCodes.Status200OK, await Get(app, OpenPath, null));
    }
}
