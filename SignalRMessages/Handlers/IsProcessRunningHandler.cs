using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MessagingAbstractions;
using OneOf;
using ReCounterDom;
using SignalRMessages.QueryRequests;
using SystemToolsShared.Errors;

namespace SignalRMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public class IsProcessRunningHandler : IQueryHandler<IsProcessRunningQueryRequest, bool>
{
    private readonly IServiceProvider _services;

    //IReCounterBackgroundTaskQueue backgroundTaskQueue, 
    // ReSharper disable once ConvertToPrimaryConstructor
    public IsProcessRunningHandler(IServiceProvider services)
    {
        _services = services;
    }

    public Task<OneOf<bool, IEnumerable<Err>>> Handle(IsProcessRunningQueryRequest request,
        CancellationToken cancellationToken)
    {
        var service = _services.GetService(typeof(ReCounterQueuedHostedService));

        if (service is null)
            throw new ArgumentNullException(nameof(service));

        // ReSharper disable once using
        using var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        return Task.FromResult(OneOf<bool, IEnumerable<Err>>.FromT0(reCounterQueuedHostedService.IsProcessRunning()));
    }
}