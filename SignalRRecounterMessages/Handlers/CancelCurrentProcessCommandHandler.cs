using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using SignalRRecounterMessages.CommandRequests;
using SystemTools.MediatRMessagingAbstractions;
using SystemTools.ReCounterAbstraction;
using SystemTools.SystemToolsShared.Errors;

namespace SignalRRecounterMessages.Handlers;

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
        CancellationToken cancellationToken = default)
    {
        var service = _services.GetService(typeof(ReCounterQueuedHostedService));

        if (service is null)
            throw new ArgumentNullException(nameof(service));

        // ReSharper disable once using
        using var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        await reCounterQueuedHostedService.StopAsync(cancellationToken);
        await reCounterQueuedHostedService.StartAsync(cancellationToken);
        return await Task.FromResult(true);
    }
}