using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using SystemTools.MediatRMessagingAbstractions;
using SystemTools.ReCounterAbstraction;
using SystemTools.SystemToolsShared.Errors;
using WebSystemTools.SignalRRecounterMessages.CommandRequests;

namespace WebSystemTools.SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CancelCurrentProcessCommandHandler : ICommandHandler<CancelCurrentProcessRequestCommand, bool>
{
    private readonly IServiceProvider _services;

    //IReCounterBackgroundTaskQueue backgroundTaskQueue, 
    // ReSharper disable once ConvertToPrimaryConstructor
    public CancelCurrentProcessCommandHandler(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<OneOf<bool, Err[]>> Handle(CancelCurrentProcessRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (_services.GetService(typeof(ReCounterQueuedHostedService)) is not ReCounterQueuedHostedService
            reCounterQueuedHostedService)
        {
            throw new InvalidOperationException(
                $"Required service {nameof(ReCounterQueuedHostedService)} is not registered.");
        }

        await reCounterQueuedHostedService.StopAsync(cancellationToken);
        await reCounterQueuedHostedService.StartAsync(cancellationToken);
        return await Task.FromResult(true);
    }
}
