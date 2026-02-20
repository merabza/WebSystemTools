using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using SystemTools.MediatRMessagingAbstractions;
using SystemTools.ReCounterAbstraction;
using SystemTools.SystemToolsShared.Errors;
using WebSystemTools.SignalRRecounterMessages.QueryRequests;

namespace WebSystemTools.SignalRRecounterMessages.Handlers;

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

    public Task<OneOf<bool, Err[]>> Handle(IsProcessRunningRequestQuery request, CancellationToken cancellationToken)
    {
        object service = _services.GetService(typeof(ReCounterQueuedHostedService)) ??
                         throw new InvalidOperationException(
                             $"Unable to resolve service of type {nameof(ReCounterQueuedHostedService)}.");

        // ReSharper disable once using
        using var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        return Task.FromResult(OneOf<bool, Err[]>.FromT0(reCounterQueuedHostedService.IsProcessRunning()));
    }
}
