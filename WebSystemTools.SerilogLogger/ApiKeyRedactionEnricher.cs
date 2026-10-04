using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog.Core;
using Serilog.Events;

namespace WebSystemTools.SerilogLogger;

//API key საიდუმლოა, მაგრამ კლიენტები მას მისამართის query-ში აგზავნიან: ApiClient — ?apikey=..., SignalR-ის
//კლიენტები — ?ApiKey=...&id=... ASP.NET Core-ის ჩანაწერები (Request starting/finished) მისამართს სრულად წერენ. ეს
//enricher-ი ჩანაწერის ყველა ტექსტურ მნიშვნელობაში, ჩადგმულშიც, ApiKey პარამეტრის მნიშვნელობას ***-ით ცვლის, ამიტომ
//გასაღები არც ერთ sink-ში არ ხვდება. დანარჩენი პარამეტრები რჩება. შეტყობინების შაბლონსა და გამონაკლისის ტექსტს
//enricher-ი ვერ ცვლის, ამიტომ მისამართი ლოგის შაბლონში ჩასმული (interpolated) ტექსტით არ უნდა ჩაიწეროს
internal sealed partial class ApiKeyRedactionEnricher : ILogEventEnricher
{
    //SystemTools.ApiContracts.ApiKeysConstants.ApiKeyParameterName. სერვერი query-ის სახელს რეგისტრის გარეშე კითხულობს
    public const string ApiKeyParameterName = "ApiKey";

    public const string RedactedValue = "***";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        List<LogEventProperty> redactedProperties =
        [
            .. logEvent.Properties.Select(p => (p.Key, p.Value, Redacted: Redact(p.Value)))
                .Where(p => !ReferenceEquals(p.Redacted, p.Value)).Select(p => new LogEventProperty(p.Key, p.Redacted))
        ];

        foreach (LogEventProperty property in redactedProperties)
        {
            logEvent.AddOrUpdateProperty(property);
        }
    }

    //ApiKey-ის მნიშვნელობა ***-ით იცვლება. პარამეტრი იწყება ტექსტის თავში ან ?, & და ჰარის შემდეგ, მნიშვნელობა კი
    //მთავრდება &-ზე, #-ზე ან ჰარეზე, რადგან მისამართი შეიძლება წინადადებაში იყოს
    public static string Redact(string text)
    {
        return ApiKeyValueRegex().Replace(text, RedactedValue);
    }

    //მნიშვნელობა იგივე ეგზემპლარად ბრუნდება, თუ მასში გასაღები არ წერია
    private static LogEventPropertyValue Redact(LogEventPropertyValue value)
    {
        switch (value)
        {
            case ScalarValue { Value: string text }:
                string redactedText = Redact(text);
                return string.Equals(redactedText, text, StringComparison.Ordinal)
                    ? value
                    : new ScalarValue(redactedText);
            case SequenceValue sequence:
                LogEventPropertyValue[] elements = [.. sequence.Elements.Select(Redact)];
                return elements.SequenceEqual(sequence.Elements, ReferenceEqualityComparer.Instance)
                    ? value
                    : new SequenceValue(elements);
            case StructureValue structure:
                LogEventProperty[] properties =
                    [.. structure.Properties.Select(p => new LogEventProperty(p.Name, Redact(p.Value)))];
                return properties.Select(p => p.Value).SequenceEqual(structure.Properties.Select(p => p.Value),
                    ReferenceEqualityComparer.Instance)
                    ? value
                    : new StructureValue(properties, structure.TypeTag);
            case DictionaryValue dictionary:
                KeyValuePair<ScalarValue, LogEventPropertyValue>[] entries =
                    [.. dictionary.Elements.Select(e => KeyValuePair.Create(e.Key, Redact(e.Value)))];
                return entries.Select(e => e.Value).SequenceEqual(dictionary.Elements.Select(e => e.Value),
                    ReferenceEqualityComparer.Instance)
                    ? value
                    : new DictionaryValue(entries);
            default:
                return value;
        }
    }

    [GeneratedRegex("(?<=(?:^|[?&\\s])" + ApiKeyParameterName + "=)[^&#\\s]*",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, 1000)]
    private static partial Regex ApiKeyValueRegex();
}
