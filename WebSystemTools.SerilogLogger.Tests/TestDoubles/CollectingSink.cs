using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace WebSystemTools.SerilogLogger.Tests.TestDoubles;

//ყოველ მიღებულ ჩანაწერს ინახავს, enricher-ების შემდეგ, ისე, როგორც მას ნამდვილი sink-ი იღებს
internal sealed class CollectingSink : ILogEventSink
{
    public List<LogEvent> Events { get; } = [];

    public void Emit(LogEvent logEvent)
    {
        Events.Add(logEvent);
    }
}
