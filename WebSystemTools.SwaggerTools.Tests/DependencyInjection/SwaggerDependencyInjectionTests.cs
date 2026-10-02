using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog;
using SystemTools.SystemToolsShared;
using WebSystemTools.SwaggerTools.DependencyInjection;

namespace WebSystemTools.SwaggerTools.Tests.DependencyInjection;

//The Swagger document that AddSwagger and UseSwaggerServices serve, read as JSON from a TestServer
public sealed class SwaggerDependencyInjectionTests
{
    private const string ApplicationName = "Test App";

    //Starts an application with AddSwagger and UseSwaggerServices on a TestServer and returns the answer to one GET
    private static async Task<(HttpStatusCode Status, string Body)> Get(
        Func<IServiceCollection, ILogger, IServiceCollection> addSwagger, ILogger? useSwaggerLogger, int versionCount,
        string path)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        addSwagger(builder.Services, new Mock<ILogger>().Object);

        await using WebApplication app = builder.Build();
        app.UseSwaggerServices(useSwaggerLogger, versionCount);
        app.MapGet("/items", () => "items");
        await app.StartAsync();

        using HttpClient client = app.GetTestClient();
        using HttpResponseMessage response = await client.GetAsync(new Uri(path, UriKind.Relative));
        string body = await response.Content.ReadAsStringAsync();
        await app.StopAsync();
        return (response.StatusCode, body);
    }

    private static async Task<JsonDocument> GetSwaggerDocument(Func<IServiceCollection, ILogger, IServiceCollection> addSwagger,
        string documentName = "v1", int versionCount = 1)
    {
        (HttpStatusCode status, string json) = await Get(addSwagger, new Mock<ILogger>().Object, versionCount,
            $"/swagger/{documentName}/swagger.json");

        Assert.Equal(HttpStatusCode.OK, status);
        return JsonDocument.Parse(json);
    }

    private static JsonElement SecurityScheme(JsonDocument document, string name)
    {
        return document.RootElement.GetProperty("components").GetProperty("securitySchemes").GetProperty(name);
    }

    //the names of the schemes that the document requires, with their scopes
    private static string[] RequiredSchemes(JsonDocument document)
    {
        return
        [
            .. document.RootElement.GetProperty("security").EnumerateArray().SelectMany(r => r.EnumerateObject())
                .Select(p => $"{p.Name}:{string.Join(',', p.Value.EnumerateArray().Select(s => s.GetString()))}")
        ];
    }

    private static bool HasSecurity(JsonDocument document)
    {
        if (document.RootElement.TryGetProperty("security", out _))
        {
            return true;
        }

        return document.RootElement.TryGetProperty("components", out JsonElement components) &&
               components.TryGetProperty("securitySchemes", out _);
    }

    [Fact]
    public async Task AddSwagger_WithApiKey_DeclaresTheApiKeyQueryParameter()
    {
        using JsonDocument document = await GetSwaggerDocument((services, logger) =>
            services.AddSwagger(logger, ESwaggerSecurityScheme.ApiKey, 1, ApplicationName));

        JsonElement scheme = SecurityScheme(document, "ApiKey");
        Assert.Equal("apiKey", scheme.GetProperty("type").GetString());
        Assert.Equal("query", scheme.GetProperty("in").GetString());
        Assert.Equal("ApiKey", scheme.GetProperty("name").GetString());
        Assert.Equal("API key in the ApiKey query parameter", scheme.GetProperty("description").GetString());
        Assert.Equal(["ApiKey:"], RequiredSchemes(document));
        Assert.False(document.RootElement.GetProperty("components").GetProperty("securitySchemes")
            .TryGetProperty("Bearer", out _));
    }

    [Fact]
    public async Task AddSwagger_WithJwtBearer_DeclaresTheAuthorizationHeader()
    {
        using JsonDocument document = await GetSwaggerDocument((services, logger) =>
            services.AddSwagger(logger, ESwaggerSecurityScheme.JwtBearer, 1, ApplicationName));

        JsonElement scheme = SecurityScheme(document, "Bearer");
        Assert.Equal("apiKey", scheme.GetProperty("type").GetString());
        Assert.Equal("header", scheme.GetProperty("in").GetString());
        Assert.Equal("Authorization", scheme.GetProperty("name").GetString());
        Assert.Equal("JWT Authorization header using the bearer scheme",
            scheme.GetProperty("description").GetString());
        Assert.Equal(["Bearer:"], RequiredSchemes(document));
    }

    [Fact]
    public async Task AddSwagger_WithNone_DeclaresNoSecurity()
    {
        using JsonDocument document = await GetSwaggerDocument((services, logger) =>
            services.AddSwagger(logger, ESwaggerSecurityScheme.None, 1, ApplicationName));

        Assert.False(HasSecurity(document));
    }

    //The old signature stays for the applications that pass true or false
    [Fact]
    public async Task AddSwagger_WithTrue_DeclaresTheJwtBearer()
    {
        using JsonDocument document = await GetSwaggerDocument((services, logger) =>
            services.AddSwagger(logger, true, 1, ApplicationName));

        Assert.Equal("header", SecurityScheme(document, "Bearer").GetProperty("in").GetString());
        Assert.Equal(["Bearer:"], RequiredSchemes(document));
    }

    [Fact]
    public async Task AddSwagger_WithFalse_DeclaresNoSecurity()
    {
        using JsonDocument document = await GetSwaggerDocument((services, logger) =>
            services.AddSwagger(logger, false, 1, ApplicationName));

        Assert.False(HasSecurity(document));
    }

    [Theory]
    [InlineData("v1")]
    [InlineData("v2")]
    public async Task AddSwagger_CreatesADocumentForEveryVersion(string documentName)
    {
        using JsonDocument document = await GetSwaggerDocument((services, logger) =>
            services.AddSwagger(logger, ESwaggerSecurityScheme.ApiKey, 2, ApplicationName), documentName, 2);

        JsonElement info = document.RootElement.GetProperty("info");
        Assert.Equal("Test App API", info.GetProperty("title").GetString());
        Assert.Equal(documentName, info.GetProperty("version").GetString());
    }

    [Fact]
    public void AddSwagger_RegistersNothing_WithoutDebugLogger()
    {
        var services = new ServiceCollection();

        IServiceCollection returned = services.AddSwagger(null, ESwaggerSecurityScheme.ApiKey);

        Assert.Same(services, returned);
        Assert.Empty(services);
    }

    [Fact]
    public void AddSwagger_LogsTheStartAndTheEnd()
    {
        var logger = new Mock<ILogger>();

        new ServiceCollection().AddSwagger(logger.Object, ESwaggerSecurityScheme.ApiKey);

        logger.Verify(l => l.Information("{MethodName} Started", "AddSwagger"), Times.Once);
        logger.Verify(l => l.Information("{MethodName} Finished", "AddSwagger"), Times.Once);
    }

    [Fact]
    public async Task AddSwagger_UsesTheMainModuleName_WithoutApplicationName()
    {
        using JsonDocument document = await GetSwaggerDocument((services, logger) =>
            services.AddSwagger(logger, ESwaggerSecurityScheme.None));

        Assert.Equal($"{StShared.GetMainModuleFileName()} API",
            document.RootElement.GetProperty("info").GetProperty("title").GetString());
    }

    [Fact]
    public async Task UseSwaggerServices_ServesNoDocument_WithoutDebugLogger()
    {
        (HttpStatusCode status, _) = await Get(
            (services, logger) => services.AddSwagger(logger, ESwaggerSecurityScheme.ApiKey, 1, ApplicationName), null,
            1, "/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.NotFound, status);
    }

    [Fact]
    public async Task UseSwaggerServices_ListsTheDocumentOfEveryVersionInTheUi()
    {
        (HttpStatusCode status, string script) = await Get(
            (services, logger) => services.AddSwagger(logger, ESwaggerSecurityScheme.ApiKey, 2, ApplicationName),
            new Mock<ILogger>().Object, 2, "/swagger/index.js");

        Assert.Equal(HttpStatusCode.OK, status);
        Assert.Contains("\"/swagger/v1/swagger.json\"", script, StringComparison.Ordinal);
        Assert.Contains("\"/swagger/v2/swagger.json\"", script, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UseSwaggerServices_ReturnsTrue_WithoutDebugLogger()
    {
        await using WebApplication app = WebApplication.CreateBuilder().Build();

        Assert.True(app.UseSwaggerServices(null));
    }

    [Fact]
    public async Task UseSwaggerServices_ReturnsTrueAndLogsTheStartAndTheEnd()
    {
        var logger = new Mock<ILogger>();
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddSwagger(logger.Object, ESwaggerSecurityScheme.None);
        await using WebApplication app = builder.Build();

        bool used = app.UseSwaggerServices(logger.Object);

        Assert.True(used);
        logger.Verify(l => l.Information("{MethodName} Started", "UseSwaggerServices"), Times.Once);
        logger.Verify(l => l.Information("{MethodName} Finished", "UseSwaggerServices"), Times.Once);
    }
}
