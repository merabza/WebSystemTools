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
public sealed class IsProcessRunningQueryHandler(IServiceProvider services)
    : IQueryHandler<IsProcessRunningRequestQuery, bool>
{
    public Task<OneOf<bool, Error[]>> Handle(IsProcessRunningRequestQuery request, CancellationToken cancellationToken)
    {
        object service = services.GetService(typeof(ReCounterQueuedHostedService)) ??
                         throw new InvalidOperationException(
                             $"Unable to resolve service of type {nameof(ReCounterQueuedHostedService)}.");

        // ReSharper disable once using
        using var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        return Task.FromResult(OneOf<bool, Error[]>.FromT0(reCounterQueuedHostedService.IsProcessRunning()));
    }
}
