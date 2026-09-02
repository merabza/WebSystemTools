using System;
using System.Threading;
using System.Threading.Tasks;
using SystemTools.Application.Abstractions.Messaging;
using SystemTools.ReCounterAbstraction;
using SystemTools.SharedKernel;
using WebSystemTools.SignalRRecounterMessages.CommandRequests;

namespace WebSystemTools.SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CancelCurrentProcessCommandHandler(IServiceProvider services)
    : ICommandHandler<CancelCurrentProcessRequestCommand, bool>
{
    public async Task<Result<bool>> Handle(CancelCurrentProcessRequestCommand request,
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
        return true;
    }
}
