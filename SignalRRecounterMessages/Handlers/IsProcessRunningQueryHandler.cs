using System;
using System.Threading;
using System.Threading.Tasks;
using MediatRMessagingAbstractions;
using OneOf;
using ReCounterAbstraction;
using SignalRRecounterMessages.QueryRequests;
using SystemToolsShared.Errors;

namespace SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IsProcessRunningQueryHandler : IQueryHandler<IsProcessRunningRequestQuery, bool>
{
    private readonly IServiceProvider _services;

    //IReCounterBackgroundTaskQueue backgroundTaskQueue, 
    // ReSharper disable once ConvertToPrimaryConstructor
    public IsProcessRunningQueryHandler(IServiceProvider services)
    {
        _services = services;
    }

    public Task<OneOf<bool, Err[]>> Handle(IsProcessRunningRequestQuery request,
        CancellationToken cancellationToken = default)
    {
        var service = _services.GetService(typeof(ReCounterQueuedHostedService));

        if (service is null)
            throw new ArgumentNullException(nameof(service));

        // ReSharper disable once using
        using var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        return Task.FromResult(OneOf<bool, Err[]>.FromT0(reCounterQueuedHostedService.IsProcessRunning()));
    }
}