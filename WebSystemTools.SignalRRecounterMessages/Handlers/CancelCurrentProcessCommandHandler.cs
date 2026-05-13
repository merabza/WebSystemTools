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
public sealed class CancelCurrentProcessCommandHandler(IServiceProvider services)
    : ICommandHandler<CancelCurrentProcessRequestCommand, bool>
{
    public async Task<OneOf<bool, Error[]>> Handle(CancelCurrentProcessRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (services.GetService(typeof(ReCounterQueuedHostedService)) is not ReCounterQueuedHostedService
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
