using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace WebSystemTools.ApiKeyIdentity.Tests.TestDoubles;

//ყველა კატეგორიისთვის ერთსა და იმავე ლოგერს აბრუნებს, რომელიც ყოველი ჩანაწერის დონესა და დაფორმატებულ ტექსტს ინახავს
internal sealed class CollectingLoggerFactory : ILoggerFactory, ILogger
{
    private readonly bool _enabled;

    public CollectingLoggerFactory(bool enabled = true)
    {
        _enabled = enabled;
    }

    public ConcurrentQueue<(LogLevel Level, string Message)> Entries { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _enabled;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Entries.Enqueue((logLevel, formatter(state, exception)));
    }

    public ILogger CreateLogger(string categoryName)
    {
        return this;
    }

    public void AddProvider(ILoggerProvider provider)
    {
    }

    public void Dispose()
    {
    }
}
