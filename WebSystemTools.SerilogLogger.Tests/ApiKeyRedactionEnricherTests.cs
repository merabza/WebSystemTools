using System;
using System.Collections.Generic;
using System.Globalization;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using SystemTools.ApiContracts;
using WebSystemTools.SerilogLogger.Tests.TestDoubles;

namespace WebSystemTools.SerilogLogger.Tests;

//The clients send the API key in the query (?apikey=..., ?ApiKey=...&id=...), and the request log of ASP.NET Core
//writes the whole query. The enricher replaces the value of the key with *** in every text of the event
public sealed class ApiKeyRedactionEnricherTests
{
    private const string Key = "made-up-key-123";

    private static LogEvent CreateEvent(params LogEventProperty[] properties)
    {
        return new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
            new MessageTemplateParser().Parse("test"), properties);
    }

    private static LogEventPropertyValue Enrich(LogEventPropertyValue value)
    {
        LogEvent logEvent = CreateEvent(new LogEventProperty("Value", value));
        new ApiKeyRedactionEnricher().Enrich(logEvent, null!);
        return logEvent.Properties["Value"];
    }

    private static object? ScalarOf(LogEventPropertyValue value)
    {
        return Assert.IsType<ScalarValue>(value).Value;
    }

    [Fact]
    public void ApiKeyParameterName_IsTheNameThatTheClientsAndTheServerUse()
    {
        Assert.Equal(ApiKeysConstants.ApiKeyParameterName, ApiKeyRedactionEnricher.ApiKeyParameterName);
    }

    [Theory]
    [InlineData("?apikey=" + Key, "?apikey=***")]
    [InlineData("?ApiKey=" + Key + "&negotiateVersion=1", "?ApiKey=***&negotiateVersion=1")]
    [InlineData("?ApiKey=" + Key + "&id=V07PgMSrAyZR9eqhV0k3CA", "?ApiKey=***&id=V07PgMSrAyZR9eqhV0k3CA")]
    [InlineData("?version=2&APIKEY=" + Key, "?version=2&APIKEY=***")]
    [InlineData("?apikey=" + Key + "#top", "?apikey=***#top")]
    [InlineData("?apikey=a%26b&x=1&apikey=" + Key, "?apikey=***&x=1&apikey=***")]
    [InlineData("apikey=" + Key, "apikey=***")]
    [InlineData("?apikey=&x=1", "?apikey=***&x=1")]
    [InlineData("GET http://localhost/api/v1/runtimes?apikey=" + Key + " - 200",
        "GET http://localhost/api/v1/runtimes?apikey=*** - 200")]
    [InlineData("query apikey=" + Key, "query apikey=***")]
    public void Redact_ReplacesTheValueOfTheApiKey(string text, string expected)
    {
        Assert.Equal(expected, ApiKeyRedactionEnricher.Redact(text));
    }

    //Only the parameter called ApiKey holds the key
    [Theory]
    [InlineData("?version=2&id=abc")]
    [InlineData("?myapikey=1&apikeys=2")]
    [InlineData("/api/v1/apikey=1")]
    [InlineData("")]
    public void Redact_KeepsATextWithoutTheApiKey(string text)
    {
        Assert.Equal(text, ApiKeyRedactionEnricher.Redact(text));
    }

    [Fact]
    public void Enrich_ReplacesTheKeyInTheQueryStringOfTheRequestLog()
    {
        LogEventPropertyValue value = Enrich(new ScalarValue("?apikey=" + Key + "&version=2"));

        Assert.Equal("?apikey=***&version=2", ScalarOf(value));
    }

    [Fact]
    public void Enrich_KeepsThePropertiesWithoutTheKey()
    {
        var queryString = new ScalarValue("?version=2");
        var statusCode = new ScalarValue(200);
        var empty = new ScalarValue(null);
        LogEvent logEvent = CreateEvent(new LogEventProperty("QueryString", queryString),
            new LogEventProperty("StatusCode", statusCode), new LogEventProperty("ContentType", empty));

        new ApiKeyRedactionEnricher().Enrich(logEvent, null!);

        Assert.Same(queryString, logEvent.Properties["QueryString"]);
        Assert.Same(statusCode, logEvent.Properties["StatusCode"]);
        Assert.Same(empty, logEvent.Properties["ContentType"]);
    }

    [Fact]
    public void Enrich_ReplacesTheKeyInEveryPropertyOfTheEvent()
    {
        LogEvent logEvent = CreateEvent(new LogEventProperty("QueryString", new ScalarValue("?apikey=" + Key)),
            new LogEventProperty("Path", new ScalarValue("/api/v1/messages")),
            new LogEventProperty("Url", new ScalarValue("http://localhost/x?ApiKey=" + Key + "&id=1")));

        new ApiKeyRedactionEnricher().Enrich(logEvent, null!);

        Assert.Equal("?apikey=***", ScalarOf(logEvent.Properties["QueryString"]));
        Assert.Equal("/api/v1/messages", ScalarOf(logEvent.Properties["Path"]));
        Assert.Equal("http://localhost/x?ApiKey=***&id=1", ScalarOf(logEvent.Properties["Url"]));
    }

    //A scope that is not a list of properties becomes the Scope sequence, a destructured object a structure
    [Fact]
    public void Enrich_ReplacesTheKeyInASequence()
    {
        var number = new ScalarValue(5);

        var sequence = Assert.IsType<SequenceValue>(Enrich(new SequenceValue([
            new ScalarValue("HTTP GET /x?apikey=" + Key), number
        ])));

        Assert.Equal("HTTP GET /x?apikey=***", ScalarOf(sequence.Elements[0]));
        Assert.Same(number, sequence.Elements[1]);
    }

    [Fact]
    public void Enrich_ReplacesTheKeyInAStructure()
    {
        var count = new ScalarValue(1);

        var structure = Assert.IsType<StructureValue>(Enrich(new StructureValue([
            new LogEventProperty("Url", new ScalarValue("/x?ApiKey=" + Key)), new LogEventProperty("Count", count)
        ], "Request")));

        Assert.Equal("Request", structure.TypeTag);
        Assert.Equal("Url", structure.Properties[0].Name);
        Assert.Equal("/x?ApiKey=***", ScalarOf(structure.Properties[0].Value));
        Assert.Equal("Count", structure.Properties[1].Name);
        Assert.Same(count, structure.Properties[1].Value);
    }

    [Fact]
    public void Enrich_ReplacesTheKeyInADictionary()
    {
        var key = new ScalarValue("url");
        var other = new ScalarValue("x");

        var dictionary = Assert.IsType<DictionaryValue>(Enrich(new DictionaryValue([
            KeyValuePair.Create<ScalarValue, LogEventPropertyValue>(key, new ScalarValue("?apikey=" + Key)),
            KeyValuePair.Create<ScalarValue, LogEventPropertyValue>(new ScalarValue("other"), other)
        ])));

        Assert.Equal("?apikey=***", ScalarOf(dictionary.Elements[key]));
        Assert.Same(other, dictionary.Elements[new ScalarValue("other")]);
    }

    [Fact]
    public void Enrich_KeepsNestedValuesWithoutTheKey()
    {
        var sequence = new SequenceValue([new ScalarValue("?version=2")]);
        var structure = new StructureValue([new LogEventProperty("Url", new ScalarValue("/x"))]);
        var dictionary = new DictionaryValue([
            KeyValuePair.Create<ScalarValue, LogEventPropertyValue>(new ScalarValue("url"), new ScalarValue("/x"))
        ]);

        Assert.Same(sequence, Enrich(sequence));
        Assert.Same(structure, Enrich(structure));
        Assert.Same(dictionary, Enrich(dictionary));
    }

    //The sinks render the message from the properties after the enrichers have run
    [Fact]
    public void Logger_WithTheEnricher_RendersTheRequestLogWithoutTheKey()
    {
        var sink = new CollectingSink();
        using Logger logger = new LoggerConfiguration().Enrich.With(new ApiKeyRedactionEnricher()).WriteTo.Sink(sink)
            .CreateLogger();

        logger.Information("Request starting {Method} {Path}{QueryString}", "GET", "/api/v1/runtimes",
            "?apikey=" + Key + "&version=2");

        string message = Assert.Single(sink.Events).RenderMessage(CultureInfo.InvariantCulture);
        Assert.DoesNotContain(Key, message, StringComparison.Ordinal);
        Assert.Contains("?apikey=***&version=2", message, StringComparison.Ordinal);
    }
}
