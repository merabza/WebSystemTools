using System;
using System.Threading;
using System.Threading.Tasks;
using SystemTools.Application.Abstractions.Messaging;
using SystemTools.ReCounterAbstraction;
using SystemTools.SharedKernel;
using WebSystemTools.SignalRRecounterMessages.QueryRequests;

namespace WebSystemTools.SignalRRecounterMessages.Handlers;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IsProcessRunningQueryHandler(IServiceProvider services)
    : IQueryHandler<IsProcessRunningRequestQuery, bool>
{
    public Task<Result<bool>> Handle(IsProcessRunningRequestQuery request, CancellationToken cancellationToken)
    {
        object service = services.GetService(typeof(ReCounterQueuedHostedService)) ??
                         throw new InvalidOperationException(
                             $"Unable to resolve service of type {nameof(ReCounterQueuedHostedService)}.");

        //ეს ჰენდლერი singleton ჰოსტინგ-სერვისის მფლობელი არ არის და მისი Dispose არ შეიძლება:
        //Dispose აუქმებს stopping token-ს და მთელი ჰოსტი ჩერდება (StopHost)
        var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        return Task.FromResult<Result<bool>>(reCounterQueuedHostedService.IsProcessRunning());
    }
}
