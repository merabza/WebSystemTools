﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatRMessagingAbstractions;
using OneOf;
using ReCounterDom;
using SignalRRecounterMessages.QueryRequests;
using SystemToolsShared.Errors;

namespace SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IsProcessRunningHandler : IQueryHandler<IsProcessRunningQueryRequest, bool>
{
    private readonly IServiceProvider _services;

    //IReCounterBackgroundTaskQueue backgroundTaskQueue, 
    // ReSharper disable once ConvertToPrimaryConstructor
    public IsProcessRunningHandler(IServiceProvider services)
    {
        _services = services;
    }

    public Task<OneOf<bool, IEnumerable<Err>>> Handle(IsProcessRunningQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var service = _services.GetService(typeof(ReCounterQueuedHostedService));

        if (service is null)
            throw new ArgumentNullException(nameof(service));

        // ReSharper disable once using
        using var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        return Task.FromResult(OneOf<bool, IEnumerable<Err>>.FromT0(reCounterQueuedHostedService.IsProcessRunning()));
    }
}